# Copilot Instructions

## Build & Run

```bash
# Build individual projects — do NOT run `dotnet build VocabularyTrainer.sln`
# The .sln includes a .sqlproj that requires Visual Studio/SSDT and fails with the dotnet CLI.
dotnet build VocabularyTrainer.Api
dotnet build VocabularyTrainer.WinApp

dotnet run --project VocabularyTrainer.Api
dotnet run --project VocabularyTrainer.WinApp

# Deploy API to local IIS
powershell -File deploy-api.ps1
```

There are no automated tests.

## Architecture

Two clients share business logic through a common layered backend:

```
WinApp  →  WinApp.ApiClient  →  Api.Contract
Api     →  BusinessLogic     →  DataAccess  →  Domain
                ↑                                  ↑
             Common                        (interfaces live here)
```

- **Domain** — entities, domain models/DTOs, repository and service interfaces, exceptions. Zero external dependencies.
- **DataAccess** — Dapper repositories hitting SQL Server. All raw SQL lives in `SqlQueries/` constants classes (e.g., `WordSqlQueries`). No ORM, no migrations.
- **BusinessLogic** — service implementations. `WordTrainerService` manages training session state via `TrainingSession`. `WordsShuffleService` orders words with `rng.NextDouble() / (weight + 1)` so lower-weight words surface more often.
- **Api** — ASP.NET Core controllers + a single `MappingProfile` (AutoMapper) that maps between `Api.Contract` DTOs and `Domain.Models`. The API only registers `DataAccess` (not `BusinessLogic`); controllers call repositories directly.
- **Api.Contract** — HTTP serialization DTOs only; shared by the API and `WinApp.ApiClient`.
- **WinApp.ApiClient** — implements the same `Domain` repository interfaces as `DataAccess`, but calls the REST API instead of SQL Server. `WinApp` is wired to call API-backed repositories via `DependencyResolver`.
- **WinApp** — MVP pattern. `MainFormPresenter` (split across partial classes per tab) holds all logic; `IMainFormView`/`MainForm` is the passive view. DI is wired in `Infrastructure/AppStart/DependencyResolver`.
- **Common** — extension helpers shared across projects (e.g., `IsNullOrEmpty`).
- **Database** — SQL Server Data Tools `.sqlproj` with schema-only SQL files. Edit these files and deploy manually for schema changes.

## Key Conventions

### Domain: entities vs models
`Domain/Entities/` — used by DataAccess and BusinessLogic (map 1:1 to DB tables).  
`Domain/Models/` — DTOs passed at service boundaries and surfaced to the API. Keep new features in the correct folder.

### Api.Contract DTOs
Strictly for HTTP wire serialization. Never reference them inside BusinessLogic or DataAccess.

### Controller using aliases
Controllers alias both `Api.Contract` and `Domain.Models` types at the top of the file to resolve naming conflicts. Follow this pattern when names clash:
```csharp
using AddWordRequest = VocabularyTrainer.Api.Contract.Words.AddWordRequest;
using DomainAddWordRequest = VocabularyTrainer.Domain.Models.AddWordRequest;
```

### Dependency injection
- `DataAccess.DependencyInjection.AddDataAccess(connectionString)` registers all repositories as singletons.
- `BusinessLogic.DependencyInjection.AddVocabularyTrainer()` registers all services as singletons and calls `AddDataAccess` internally.
- The API only calls `AddDataAccess`; WinApp calls `AddVocabularyTrainer` (which uses ApiClient repositories).

### WinApp presenter error handling
All async presenter actions are wrapped in `ExecuteIfFreeAsync`, which sets a `_isBusy` guard, catches `DuplicateNameException`/`DatabaseException`/`Exception`, and surfaces errors via `_view.ShowError(...)`. User-facing string constants live in `Infrastructure/Constants.cs`.

### Database
SQL Server only. Default connection string:
```
Server=localhost;Database=VocabularyTrainer;User Id=VTLogin;Password=VTPass;TrustServerCertificate=True;
```
Key schema: `Users` → `Dictionaries` (1:many). `UserWords` is the junction of `Users + Words + Dictionaries` and holds the `Weight` column (0–`WordConstants.MaxWeight` = 10).
