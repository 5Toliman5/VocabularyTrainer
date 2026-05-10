#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Publishes VocabularyTrainer.Api and deploys it to a local IIS site.

.DESCRIPTION
    - Stops the IIS site and pool, then waits until the pool is actually Stopped (AlwaysRunning
      / in-process hosting otherwise keeps DLLs locked under inetpub).
    - Publishes to a temp folder (MSBuild node reuse disabled so staging DLLs are not held).
    - Stops all of IIS (iisreset /stop) so inetpub and staging are not locked by w3wp, mirrors
      into the physical path, then starts IIS again.
    - Creates the IIS application pool and site if they do not exist; updates them otherwise.
    - Starts everything back up after deployment.

.PREREQUISITES
    - IIS installed (Control Panel > Programs > Turn Windows features on or off > IIS).
    - ASP.NET Core Hosting Bundle installed (https://dotnet.microsoft.com/download).
    - Run this script as Administrator.
    - Brief full-IIS outage during copy: iisreset /stop then /start so every site on this machine
      drops w3wp file locks (required for reliable in-process redeploys to inetpub).

.NOTES
    Uses appcmd.exe instead of the WebAdministration PowerShell module. The IIS: PSDrive is
    unreliable in PowerShell 7 (the WinPS compat session does not always project the provider
    drive locally), so we drive IIS directly via appcmd, which works the same on Windows
    PowerShell and PowerShell 7.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$env:MSBUILDDISABLENODEREUSE = "1"

# ---------------------------------------------------------------------------
# Configuration — adjust these if needed
# ---------------------------------------------------------------------------
$AppPoolName = "VocabularyTrainerApi"
$SiteName    = "VocabularyTrainerApi"
$Port        = 8080
$DeployPath  = "C:\inetpub\VocabularyTrainerApi"
$ProjectPath = Join-Path $PSScriptRoot "VocabularyTrainer.Api\VocabularyTrainer.Api.csproj"
$AspNetCoreEnvironment = "Development"
$StagingPath = Join-Path ([System.IO.Path]::GetTempPath()) "VocabularyTrainerApi_publish"
$AppPoolStopTimeoutSec = 120

$AppCmd = Join-Path $env:windir "System32\inetsrv\appcmd.exe"
$IisReset = Join-Path $env:windir "System32\iisreset.exe"
# ---------------------------------------------------------------------------

function Write-Step([string]$Message) {
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Invoke-AppCmd {
    param(
        [Parameter(Mandatory = $true)][string[]]$Arguments,
        [switch]$AllowFailure
    )

    $output = & $AppCmd @Arguments 2>&1
    $exit = $LASTEXITCODE
    if (-not $AllowFailure -and $exit -ne 0) {
        $joined = ($Arguments -join ' ')
        $text = ($output | Out-String).Trim()
        throw "appcmd failed (exit $exit): $joined`n$text"
    }

    return $output
}

function Test-IisAppPoolExists {
    param([Parameter(Mandatory = $true)][string]$Name)

    $null = & $AppCmd list apppool "$Name" 2>&1
    return $LASTEXITCODE -eq 0
}

function Get-IisAppPoolState {
    param([Parameter(Mandatory = $true)][string]$Name)

    $output = & $AppCmd list apppool "$Name" /text:state 2>$null
    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    $value = ($output | Out-String).Trim()
    if ([string]::IsNullOrEmpty($value)) {
        return $null
    }

    return $value
}

function Test-IisSiteExists {
    param([Parameter(Mandatory = $true)][string]$Name)

    $null = & $AppCmd list site "$Name" 2>&1
    return $LASTEXITCODE -eq 0
}

function Get-IisSiteBindings {
    param([Parameter(Mandatory = $true)][string]$Name)

    $output = & $AppCmd list site "$Name" /text:bindings 2>$null
    if ($LASTEXITCODE -ne 0) {
        return @()
    }

    $value = ($output | Out-String).Trim()
    if ([string]::IsNullOrEmpty($value)) {
        return @()
    }

    return ($value -split ',') | ForEach-Object { $_.Trim() } | Where-Object { $_ }
}

function Wait-IisAppPoolStopped {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][int]$TimeoutSec
    )

    if (-not (Test-IisAppPoolExists -Name $Name)) {
        return
    }

    $deadline = (Get-Date).AddSeconds($TimeoutSec)
    while ((Get-Date) -lt $deadline) {
        $state = Get-IisAppPoolState -Name $Name
        if ($state -eq "Stopped") {
            return
        }

        Start-Sleep -Milliseconds 400
    }

    $finalState = Get-IisAppPoolState -Name $Name
    throw "Application pool '$Name' did not stop within ${TimeoutSec}s (last state: $finalState)."
}

function Set-PublishedWebConfigAspNetCoreEnvironment {
    param(
        [Parameter(Mandatory = $true)][string]$DeployPath,
        [Parameter(Mandatory = $true)][string]$EnvironmentName
    )

    $webConfigPath = Join-Path $DeployPath "web.config"
    if (-not (Test-Path -LiteralPath $webConfigPath)) {
        throw "web.config not found after publish: $webConfigPath"
    }

    [xml]$doc = Get-Content -LiteralPath $webConfigPath
    $aspNetCore = $doc.SelectSingleNode("/configuration/location/system.webServer/aspNetCore")
    if ($null -eq $aspNetCore) {
        throw "aspNetCore element not found in web.config."
    }

    $environmentVariables = $aspNetCore.SelectSingleNode("environmentVariables")
    if ($null -eq $environmentVariables) {
        $environmentVariables = $doc.CreateElement("environmentVariables")
        [void]$aspNetCore.AppendChild($environmentVariables)
    }

    $existing = $environmentVariables.SelectSingleNode("environmentVariable[@name='ASPNETCORE_ENVIRONMENT']")
    if ($null -ne $existing) {
        $existing.SetAttribute("value", $EnvironmentName)
    }
    else {
        $newVar = $doc.CreateElement("environmentVariable")
        $newVar.SetAttribute("name", "ASPNETCORE_ENVIRONMENT")
        $newVar.SetAttribute("value", $EnvironmentName)
        [void]$environmentVariables.AppendChild($newVar)
    }

    $doc.Save($webConfigPath)
}

# --- Preflight: ASP.NET Core Module v2 (ANCM) ---
Write-Step "Checking ASP.NET Core Module registration..."
$ancmCandidates = @(
    "$env:SystemRoot\System32\inetsrv\aspnetcorev2.dll",
    "$env:ProgramFiles\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
)
$ancmPath = $ancmCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $ancmPath) {
    Write-Host ""
    Write-Host "ERROR: ASP.NET Core Module v2 is not installed." -ForegroundColor Red
    Write-Host "Install the .NET 10 Hosting Bundle, then run 'iisreset' and re-run this script." -ForegroundColor Yellow
    Write-Host "Download: https://dotnet.microsoft.com/en-us/download/dotnet/10.0"
    exit 1
}
Write-Host "  ANCM v2 found: $ancmPath" -ForegroundColor Green

# --- Preflight: appcmd available ---
if (-not (Test-Path -LiteralPath $AppCmd)) {
    throw "appcmd.exe not found at '$AppCmd'. Is the IIS Management feature installed?"
}

# --- Stop site and pool so inetpub is not locked; wait until pool is really Stopped ---
if (Test-IisSiteExists -Name $SiteName) {
    Write-Step "Stopping site '$SiteName'..."
    Invoke-AppCmd -Arguments @('stop', 'site', $SiteName) -AllowFailure | Out-Null
}
if (Test-IisAppPoolExists -Name $AppPoolName) {
    Write-Step "Stopping application pool '$AppPoolName'..."
    Invoke-AppCmd -Arguments @('stop', 'apppool', $AppPoolName) -AllowFailure | Out-Null

    Write-Step "Waiting for application pool '$AppPoolName' to stop (release file locks)..."
    Wait-IisAppPoolStopped -Name $AppPoolName -TimeoutSec $AppPoolStopTimeoutSec
    Start-Sleep -Seconds 1
}

# --- Publish to staging (avoids publishing into inetpub while w3wp is still shutting down) ---
if (Test-Path -LiteralPath $StagingPath) {
    Remove-Item -LiteralPath $StagingPath -Recurse -Force
}

Write-Step "Publishing in Release mode (staging)..."
dotnet publish $ProjectPath --configuration Release --output $StagingPath --no-self-contained
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

Write-Step "Setting ASPNETCORE_ENVIRONMENT in staged web.config..."
Set-PublishedWebConfigAspNetCoreEnvironment -DeployPath $StagingPath -EnvironmentName $AspNetCoreEnvironment

if (-not (Test-Path -LiteralPath $DeployPath)) {
    New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
}

Write-Step "Stopping IIS (iisreset /stop) so w3wp releases DLLs under inetpub and temp..."
& $IisReset /stop
if ($LASTEXITCODE -ne 0) {
    throw "iisreset /stop failed (exit $LASTEXITCODE). Run the script from an elevated shell."
}

try {
    Write-Step "Copying staged build to '$DeployPath'..."
    & robocopy.exe $StagingPath $DeployPath /MIR /R:12 /W:2 /NDL /NFL /NJH /NJS /nc /ns /np
    $robocopyExit = $LASTEXITCODE
    if ($robocopyExit -ge 8) {
        throw "robocopy failed (exit $robocopyExit) mirroring staging to '$DeployPath'."
    }
}
finally {
    Write-Step "Starting IIS (iisreset /start)..."
    & $IisReset /start
    if ($LASTEXITCODE -ne 0) {
        throw "iisreset /start failed (exit $LASTEXITCODE). Start W3SVC manually, then fix the site."
    }

    Start-Sleep -Seconds 3
}

Remove-Item -LiteralPath $StagingPath -Recurse -Force -ErrorAction SilentlyContinue

# --- Application pool ---
if (-not (Test-IisAppPoolExists -Name $AppPoolName)) {
    Write-Step "Creating application pool '$AppPoolName'..."
    Invoke-AppCmd -Arguments @('add', 'apppool', "/name:$AppPoolName") | Out-Null
}
else {
    Write-Step "Updating application pool '$AppPoolName'..."
}

# ASP.NET Core runs its own runtime — pool must be set to No Managed Code.
# AlwaysRunning + idleTimeout=0 keep the worker warm.
Invoke-AppCmd -Arguments @(
    'set', 'apppool', $AppPoolName,
    '/managedRuntimeVersion:',
    '/startMode:AlwaysRunning',
    '/processModel.idleTimeout:00:00:00'
) | Out-Null

# --- Site ---
if (-not (Test-IisSiteExists -Name $SiteName)) {
    Write-Step "Creating IIS site '$SiteName' on port $Port..."
    Invoke-AppCmd -Arguments @(
        'add', 'site',
        "/name:$SiteName",
        "/bindings:http/*:${Port}:",
        "/physicalPath:$DeployPath"
    ) | Out-Null

    Invoke-AppCmd -Arguments @(
        'set', 'app', "$SiteName/",
        "/applicationPool:$AppPoolName"
    ) | Out-Null
}
else {
    Write-Step "Updating IIS site '$SiteName'..."
    Invoke-AppCmd -Arguments @(
        'set', 'vdir', "$SiteName/",
        "/physicalPath:$DeployPath"
    ) | Out-Null

    Invoke-AppCmd -Arguments @(
        'set', 'app', "$SiteName/",
        "/applicationPool:$AppPoolName"
    ) | Out-Null

    $expectedBinding = "http/*:${Port}:"
    $bindings = Get-IisSiteBindings -Name $SiteName

    $hasExpected = $false
    foreach ($binding in $bindings) {
        if ($binding -eq $expectedBinding) {
            $hasExpected = $true
            break
        }
    }

    if (-not $hasExpected) {
        foreach ($binding in $bindings) {
            if ($binding -like 'http/*') {
                $info = $binding.Substring(5)
                Invoke-AppCmd -Arguments @(
                    'set', 'site', $SiteName,
                    "/-bindings.[protocol='http',bindingInformation='$info']"
                ) -AllowFailure | Out-Null
            }
        }

        Invoke-AppCmd -Arguments @(
            'set', 'site', $SiteName,
            "/+bindings.[protocol='http',bindingInformation='*:${Port}:']"
        ) | Out-Null
    }
}

# --- Start ---
Write-Step "Starting application pool '$AppPoolName'..."
Invoke-AppCmd -Arguments @('start', 'apppool', $AppPoolName) -AllowFailure | Out-Null

Write-Step "Starting site '$SiteName'..."
Invoke-AppCmd -Arguments @('start', 'site', $SiteName) -AllowFailure | Out-Null

Write-Host ""
Write-Host "Deployment complete." -ForegroundColor Green
Write-Host "  API:     http://localhost:$Port"
Write-Host "  Swagger: http://localhost:$Port/swagger"
