# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Build C# projects (do NOT use VocabularyTrainer.sln — it includes the .sqlproj which requires Visual Studio/SSDT and fails with the dotnet CLI)
dotnet build VocabularyTrainer.Api
dotnet build VocabularyTrainer.WinApp

# Run the REST API (port 8080 by default in IIS; dev port depends on launchSettings.json)
dotnet run --project VocabularyTrainer.Api

# Run the WinForms desktop app
dotnet run --project VocabularyTrainer.WinApp

# Deploy API to local IIS
powershell -File deploy-api.ps1
```

There are no automated tests in this solution.

## Architecture

This is a two-client vocabulary training app: a **REST API** (`VocabularyTrainer.Api`) and a **WinForms desktop app** (`VocabularyTrainer.WinApp`). The desktop app calls the API via `VocabularyTrainer.WinApp.ApiClient`.

**Layer dependency order (top → bottom):**

```
WinApp  →  ApiClient  →  Api.Contract
Api     →  BusinessLogic  →  DataAccess  →  Domain
                ↑                               ↑
         Common / Common.Web               (interfaces live here)
```

- **Domain** — entities (`User`, `Word`, `UserWord`, `UserDictionary`), domain models/DTOs, repository and service interfaces, and exceptions. No external dependencies.
- **BusinessLogic** — service implementations. Key classes: `WordTrainerService` (training session orchestration), `WordsShuffleService` (weight-based probability shuffling), `TrainingSession` (active session state).
- **DataAccess** — Dapper repositories against SQL Server. Raw SQL is stored in `SqlQueries` constants classes. No ORM/migrations — schema lives in the `.sqlproj`.
- **Api** — ASP.NET Core controllers (`Users`, `Words`, `Dictionaries`) + AutoMapper profile mapping Api.Contract DTOs ↔ Domain models.
- **Api.Contract** — HTTP request/response DTOs shared between the API and the API client library.
- **WinApp** — MVP pattern. `MainFormPresenter` holds all UI logic; `IMainFormView` / `MainForm` is the passive view. DI is wired in `AppStart/DependencyResolver`.
- **Database** — SQL Server Data Tools `.sqlproj` with four schema files: `Users`, `Words`, `Dictionaries`, `UserWords`.
- **Common** — Universal class library (future NuGet). Contains `Result`/`Result<T>` wrappers for non-HTTP contexts (WinForms, services).
- **Common.Web** — Web-specific class library (future NuGet). References Common. Contains `ApiResult`/`ApiResult<T>` with `ApiErrorType` enum, and `BaseApiController` with `ResolveFailure`. Only referenced by web API projects.

## Core Domain Concept: Weight-Based Training

`UserWords.Weight` (0–`WordConstants.MAX_WEIGHT`) represents how well the user knows a word. `WordsShuffleService` orders words so lower-weight (less-known) words appear more often. Training sessions increase or decrease weight based on user responses.

## Database

SQL Server only. Connection string:
```
Server=localhost;Database=VocabularyTrainer;User Id=VTLogin;Password=VTPass;TrustServerCertificate=True;
```

Schema changes are made by editing SQL files in `VocabularyTrainer.Database/` and deploying manually — there is no migration framework.

Key relationships: `Users` → `Dictionaries` (one-to-many), `Users` + `Words` + `Dictionaries` → `UserWords` (junction with `Weight`). A word's translation lives on `Words`; per-user metadata lives on `UserWords`.

## Naming Conventions

- The `Domain` project has two parallel naming spaces: **entities** (used by DataAccess/BusinessLogic) and **models** (DTOs passed around at service boundaries and surfaced to the API). When adding new features, keep entities in `Domain/Entities/` and request/response models in `Domain/Models/`.
- Api.Contract DTOs are only for HTTP serialization — do not reuse them inside BusinessLogic or DataAccess.

## Error Handling Pattern

**Rule: throw for unexpected errors, return `Result` for expected failures.**

### What counts as expected vs unexpected

| Situation | Pattern |
|---|---|
| Duplicate name constraint violation | Return `Result.Failure(...)` |
| Entity not found (user, dictionary) | Return `Result.Failure(...)` |
| Validation failures | Return `ApiResult.Failure(..., ApiErrorType.Validation)` in controllers |
| DB connection refused / SQL timeout | Throw `DatabaseException` |
| Unexpected HTTP error from the API | Throw via `EnsureSuccessStatusCode()` |
| Any other unexpected exception | Let it propagate (caught by global handler) |

### Result types

- **`Common.Wrappers.Result` / `Result<T>`** — base result types. Used at domain service and repository boundaries. No HTTP concepts. Suitable for WinForms and any non-web context.
- **`Common.Web.Wrappers.ApiResult` / `ApiResult<T>`** — extends the base results with `ApiErrorType`. Used only at the API controller layer to describe HTTP failure semantics.
- **`ApiErrorType` enum** — `Validation` (400), `NotFound` (404), `Conflict` (409), `Unauthorized` (401), `Forbidden` (403).

### Repository and service interfaces

Repository methods that can produce expected business failures return `Result`/`Result<T>`:
- `IDictionaryRepository.AddAsync` → `Task<Result<int>>` (can fail with duplicate name)
- `IDictionaryRepository.UpdateAsync` → `Task<Result>` (can fail with duplicate name)
- `IUserRepository.GetUserAsync` → `Task<Result<UserModel>>` (can fail with not found)

Service interfaces mirror repository Result returns and propagate them up.

Repositories always wrap external calls (SQL, HTTP) in try-catch:
- **DataAccess**: catch `SqlException` for duplicate-key → return `Result.Failure`; other `SqlException` → throw `DatabaseException`.
- **WinApp.ApiClient**: check HTTP status code → return `Result.Failure` for expected codes (409, 404); call `EnsureSuccessStatusCode()` for unexpected failures.

### API controllers

All controllers inherit `BaseApiController` (from `Common.Web`).

Controllers convert domain `Result` failures to `ApiResult` with the appropriate `ErrorType`, then call `ResolveFailure`:

```csharp
var result = await repository.AddAsync(request);
if (!result.Successful)
    return ResolveFailure(ApiResult.Failure(result.ErrorMessage!, ApiErrorType.Conflict));
```

There is no per-endpoint try-catch. `GlobalExceptionHandlerMiddleware` in `VocabularyTrainer.Api/Middleware/` catches all unhandled exceptions, logs them, and returns a generic 500 `ProblemDetails` response.

### WinApp presenter

`ExecuteIfFreeAsync` catches `DatabaseException` (unexpected DB failure) and `Exception` (any other unexpected). Expected failures (duplicate name, user not found) are returned as `Result` from services and checked inline:

```csharp
var result = await _dictionaryService.AddAsync(request);
if (!result.Successful)
{
    _view.ShowError(Constants.DuplicateDictionaryName);
    return;
}
```

User loading uses the same pattern with `Result<UserModel>` from `IUserService.GetAsync`.
