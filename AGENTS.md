# AGENTS.md

Shared project description and conventions for AI coding assistants (Claude Code, GitHub Copilot, Cursor, etc.). All tool-specific instruction files (`CLAUDE.md`, `.github/copilot-instructions.md`, `.cursorrules`) point to this document.

## Build & Run

```bash
# Build C# projects (do NOT use VocabularyTrainer.sln — it includes the .sqlproj
# which requires Visual Studio/SSDT and fails with the dotnet CLI)
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

A vocabulary training app with two clients sharing a layered backend: a **REST API** (`VocabularyTrainer.Api`) and a **WinForms desktop app** (`VocabularyTrainer.WinApp`). The desktop app talks to the API via `VocabularyTrainer.WinApp.ApiClient`.

The API is designed to be client-agnostic: in the future it may be consumed by additional front-ends such as a Telegram bot, a mobile app, or a web SPA. Keep API-facing concerns (DTOs, error contracts, authentication) generic — do not bake in WinApp-specific assumptions.

**Layer dependency order (top → bottom):**

```
WinApp  →  WinApp.ApiClient  →  Api.Contract
Api     →  Api.BusinessLogic →  BusinessLogic  →  DataAccess  →  Domain
                ↑                      ↑                            ↑
         Common.Web                 Common                  (interfaces live here)
```

### Projects

- **Domain** — entities (`User`, `Word`, `UserWord`, `UserDictionary`), domain models/DTOs, repository and service interfaces, exceptions. Zero external dependencies.
- **DataAccess** — Dapper repositories against SQL Server. Raw SQL lives in `SqlQueries/` constants classes (e.g., `WordSqlQueries`). No ORM, no migrations.
- **BusinessLogic** — domain service implementations. Key classes: `WordTrainerService` (training session orchestration), `WordsShuffleService` (weight-based probability shuffling using `rng.NextDouble() / (weight + 1)`), `TrainingSession` (active session state). Returns `Result`/`Result<T>`.
- **Api.BusinessLogic** — API-flavored service layer that wraps the domain services and returns `ApiOperationResult`/`ApiOperationResult<T>` (with `ApiErrorType`). Registered via `AddApiBusinessLogic(connectionString)`, which internally calls `AddVocabularyTrainer` from `BusinessLogic`.
- **Api** — ASP.NET Core controllers (`Users`, `Words`, `Dictionaries`) + a single `MappingProfile` (AutoMapper) mapping `Api.Contract` DTOs ↔ `Domain.Models`. Controllers depend on `Api.BusinessLogic` services (e.g., `IApiDictionaryService`), **never** on repositories or `BusinessLogic` services directly.
- **Api.Contract** — HTTP request/response DTOs only; shared by the API and `WinApp.ApiClient`.
- **WinApp.ApiClient** — implements the same `Domain` repository interfaces as `DataAccess`, but calls the REST API instead of SQL Server. WinApp is wired to use these API-backed repositories via `DependencyResolver`.
- **WinApp** — MVP pattern. `MainFormPresenter` (split across partial classes per tab — `MyWords`, `MyDictionaries`, `AddWord`, `TrainYourself`) holds all UI logic; `IMainFormView`/`MainForm` is the passive view. DI is wired in `Infrastructure/AppStart/DependencyResolver`. User-facing strings live in `Infrastructure/Constants.cs`.
- **Common** — universal class library (future NuGet). Contains `Result`/`Result<T>` wrappers and shared extensions (e.g., `IsNullOrEmpty`). No web concepts.
- **Common.Web** — web-specific class library (future NuGet). References `Common`. Contains `ApiOperationResult`/`ApiOperationResult<T>`, `ApiErrorType` enum, and `BaseApiController` with `ResolveFailure`. Only referenced by web API projects.
- **Database** — SQL Server Data Tools `.sqlproj` with four schema files: `Users`, `Words`, `Dictionaries`, `UserWords`. Edit these files and deploy manually for schema changes.

## Core Domain Concept: Weight-Based Training

`UserWords.Weight` (0–`WordConstants.MAX_WEIGHT`) represents how well the user knows a word. `WordsShuffleService` orders words so lower-weight (less-known) words appear more often. Training sessions increase or decrease weight based on user responses.

## Database

SQL Server only. The connection string lives in [`VocabularyTrainer.Api/appsettings.json`](VocabularyTrainer.Api/appsettings.json) under `ConnectionStrings:Default` — read it from there if needed; do not duplicate credentials in documentation.

Schema changes are made by editing SQL files in `VocabularyTrainer.Database/` and deploying manually — there is no migration framework.

Key relationships: `Users` → `Dictionaries` (one-to-many); `UserWords` is the junction of `Users + Words + Dictionaries` and holds the `Weight` column. A word's translation lives on `Words`; per-user metadata lives on `UserWords`.

## Naming Conventions

### Domain: entities vs models

The `Domain` project has two parallel naming spaces:

- `Domain/Entities/` — used by `DataAccess` and `BusinessLogic` (map 1:1 to DB tables).
- `Domain/Models/` — DTOs passed at service boundaries and surfaced to the API.

Keep new features in the correct folder.

### Api.Contract DTOs

Strictly for HTTP wire serialization. **Never** reference them inside `BusinessLogic`, `Api.BusinessLogic`, or `DataAccess`.

### Controller using-aliases

Controllers alias both `Api.Contract` and `Domain.Models` types at the top of the file when names clash:

```csharp
using AddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using DomainAddWordRequest = VocabularyTrainer.Domain.Models.AddWordRequest;
```

### Dependency injection

- `DataAccess.AddDataAccess(connectionString)` — registers all repositories as singletons.
- `BusinessLogic.AddVocabularyTrainer(connectionString)` — registers domain services as singletons and calls `AddDataAccess` internally.
- `Api.BusinessLogic.AddApiBusinessLogic(connectionString)` — registers API services and calls `AddVocabularyTrainer` internally. **This is what the API uses.**
- WinApp wires API-backed repositories via `DependencyResolver` + `ServiceCollectionExtensions`.

## Error Handling Pattern

**Rule: throw for unexpected errors, return a `Result` for expected failures.**

### What counts as expected vs unexpected

| Situation                            | Pattern                                                    |
| ------------------------------------ | ---------------------------------------------------------- |
| Duplicate name constraint violation  | Return `Result.Failure(...)`                               |
| Entity not found (user, dictionary)  | Return `Result.Failure(...)`                               |
| Validation failures                  | Return `ApiOperationResult.Failure(..., ApiErrorType.Validation)` in API services |
| DB connection refused / SQL timeout  | Throw `DatabaseException`                                  |
| Unexpected HTTP error from the API   | Throw via `EnsureSuccessStatusCode()`                      |
| Any other unexpected exception       | Let it propagate (caught by global handler / presenter)    |

### Result types

- **`Common.Wrappers.Result` / `Result<T>`** — base result types. Used at domain service and repository boundaries. No HTTP concepts. Suitable for WinForms and any non-web context.
- **`Common.Web.Wrappers.ApiOperationResult` / `ApiOperationResult<T>`** — extends the base results with `ApiErrorType`. Used at the `Api.BusinessLogic` service layer and consumed by API controllers.
- **`ApiErrorType` enum** — `Validation` (400), `NotFound` (404), `Conflict` (409), `Unauthorized` (401), `Forbidden` (403).

### Repository and service interfaces

Repository methods that can produce expected business failures return `Result`/`Result<T>`:

- `IDictionaryRepository.AddAsync` → `Task<Result<int>>` (can fail with duplicate name)
- `IDictionaryRepository.UpdateAsync` → `Task<Result>` (can fail with duplicate name)
- `IUserRepository.GetUserAsync` → `Task<Result<UserModel>>` (can fail with not found)

Domain service interfaces (in `BusinessLogic`) mirror repository `Result` returns and propagate them up. `Api.BusinessLogic` services translate `Result` → `ApiOperationResult` with the appropriate `ApiErrorType`.

Repositories always wrap external calls (SQL, HTTP) in try-catch:

- **DataAccess**: catch `SqlException` for duplicate-key → return `Result.Failure`; other `SqlException` → throw `DatabaseException`.
- **WinApp.ApiClient**: check HTTP status code → return `Result.Failure` for expected codes (409, 404); call `EnsureSuccessStatusCode()` for unexpected failures.

### API controllers

All controllers inherit `BaseApiController` (from `Common.Web`). Controllers depend on `Api.BusinessLogic` services (e.g., `IApiDictionaryService`) and use `IMapper` to translate between `Api.Contract` DTOs and `Domain.Models`.

Because `Api.BusinessLogic` already returns `ApiOperationResult`, controllers call `ResolveFailure` directly:

```csharp
var result = await service.AddAsync(mapper.Map<DomainAddDictRequest>(request));
if (!result.Successful)
    return ResolveFailure(result);

return CreatedAtAction(...);
```

There is no per-endpoint try-catch. `GlobalExceptionHandlerMiddleware` in `VocabularyTrainer.Api/Middleware/` catches all unhandled exceptions, logs them, and returns a generic 500 `ProblemDetails` response.

### WinApp presenter

All async presenter actions are wrapped in `ExecuteIfFreeAsync`, which sets an `_isBusy` guard and catches:

- `DatabaseException` → `_view.ShowError(Constants.DatabaseError)`
- `Exception` (catch-all) → `_view.ShowError(string.Format(Constants.UnexpectedError, ex.Message))`

Expected failures (duplicate name, user not found) come back as `Result` from services and are checked inline:

```csharp
var result = await _dictionaryService.AddAsync(request);
if (!result.Successful)
{
    _view.ShowError(Constants.DuplicateDictionaryName);
    return;
}
```

User loading uses the same pattern with `Result<UserModel>` from `IUserService.GetAsync`.
