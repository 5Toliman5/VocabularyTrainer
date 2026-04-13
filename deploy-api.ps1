#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Publishes VocabularyTrainer.Api and deploys it to a local IIS site.

.DESCRIPTION
    - Stops the IIS site and pool first (releases file locks).
    - Publishes the project in Release mode.
    - Creates the IIS application pool and site if they do not exist.
    - Starts everything back up after deployment.

.PREREQUISITES
    - IIS installed (Control Panel > Programs > Turn Windows features on or off > IIS).
    - ASP.NET Core Hosting Bundle installed (https://dotnet.microsoft.com/download).
    - Run this script as Administrator.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ---------------------------------------------------------------------------
# Configuration — adjust these if needed
# ---------------------------------------------------------------------------
$AppPoolName = "VocabularyTrainerApi"
$SiteName    = "VocabularyTrainerApi"
$Port        = 8080
$DeployPath  = "C:\inetpub\VocabularyTrainerApi"
$ProjectPath = Join-Path $PSScriptRoot "VocabularyTrainer.Api\VocabularyTrainer.Api.csproj"
# ---------------------------------------------------------------------------

function Write-Step([string]$Message) {
    Write-Host "`n==> $Message" -ForegroundColor Cyan
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

# --- Load IIS module early so we can stop the pool before publishing ---
Write-Step "Loading IIS PowerShell module..."
Import-Module WebAdministration -ErrorAction Stop

# --- Stop site and pool BEFORE publishing to release file locks ---
if (Test-Path "IIS:\Sites\$SiteName") {
    Write-Step "Stopping site '$SiteName'..."
    Stop-Website -Name $SiteName -ErrorAction SilentlyContinue
}
if (Test-Path "IIS:\AppPools\$AppPoolName") {
    Write-Step "Stopping application pool '$AppPoolName'..."
    Stop-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

# --- Publish ---
Write-Step "Publishing in Release mode..."
dotnet publish $ProjectPath --configuration Release --output $DeployPath --no-self-contained
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

# --- Application pool ---
if (-not (Test-Path "IIS:\AppPools\$AppPoolName")) {
    Write-Step "Creating application pool '$AppPoolName'..."
    New-WebAppPool -Name $AppPoolName | Out-Null
} else {
    Write-Step "Updating application pool '$AppPoolName'..."
}
# ASP.NET Core runs its own runtime — pool must be set to No Managed Code
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion    -Value ""
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name startMode                -Value "AlwaysRunning"
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name processModel.idleTimeout -Value "00:00:00"

# --- Site ---
if (-not (Test-Path "IIS:\Sites\$SiteName")) {
    Write-Step "Creating IIS site '$SiteName' on port $Port..."
    New-Website -Name $SiteName `
                -Port $Port `
                -PhysicalPath $DeployPath `
                -ApplicationPool $AppPoolName | Out-Null
} else {
    Write-Step "Updating IIS site '$SiteName'..."
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name physicalPath    -Value $DeployPath
    Set-ItemProperty "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName

    # Re-create the HTTP binding if the port changed
    $existing = Get-WebBinding -Name $SiteName -Protocol "http" -ErrorAction SilentlyContinue
    if ($null -eq $existing -or $existing.bindingInformation -ne "*:${Port}:") {
        Remove-WebBinding -Name $SiteName -Protocol "http" -ErrorAction SilentlyContinue
        New-WebBinding -Name $SiteName -Protocol "http" -Port $Port -IPAddress "*" | Out-Null
    }
}

# --- Environment variables ---
Write-Step "Configuring environment variables..."
Set-WebConfigurationProperty -PSPath "IIS:\Sites\$SiteName" `
    -Filter "system.webServer/aspNetCore/environmentVariables" `
    -Name "." `
    -Value @{ name = "ASPNETCORE_ENVIRONMENT"; value = "Development" }

# --- Start ---
Write-Step "Starting application pool '$AppPoolName'..."
Start-WebAppPool -Name $AppPoolName

Write-Step "Starting site '$SiteName'..."
Start-Website -Name $SiteName

Write-Host ""
Write-Host "Deployment complete." -ForegroundColor Green
Write-Host "  API:     http://localhost:$Port"
Write-Host "  Swagger: http://localhost:$Port/swagger"
