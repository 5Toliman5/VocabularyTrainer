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
              Common                        (interfaces live here)
```

- **Domain** — entities (`User`, `Word`, `UserWord`, `UserDictionary`), domain models/DTOs, repository and service interfaces, and exceptions. No external dependencies.
- **BusinessLogic** — service implementations. Key classes: `WordTrainerService` (training session orchestration), `WordsShuffleService` (weight-based probability shuffling), `TrainingSession` (active session state).
- **DataAccess** — Dapper repositories against SQL Server. Raw SQL is stored in `SqlQueries` constants classes. No ORM/migrations — schema lives in the `.sqlproj`.
- **Api** — ASP.NET Core controllers (`Users`, `Words`, `Dictionaries`) + AutoMapper profile mapping Api.Contract DTOs ↔ Domain models.
- **Api.Contract** — HTTP request/response DTOs shared between the API and the API client library.
- **WinApp** — MVP pattern. `MainFormPresenter` holds all UI logic; `IMainFormView` / `MainForm` is the passive view. DI is wired in `AppStart/DependencyResolver`.
- **Database** — SQL Server Data Tools `.sqlproj` with four schema files: `Users`, `Words`, `Dictionaries`, `UserWords`.

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
