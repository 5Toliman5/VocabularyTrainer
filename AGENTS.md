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

### Secrets

Secrets are **never** stored in `appsettings.json` or anywhere in the repository. The API reads them from configuration keys that come from:

- **Development:** .NET user-secrets of `VocabularyTrainer.Api` (`UserSecretsId` in the `.csproj`; loaded automatically when `ASPNETCORE_ENVIRONMENT=Development`, which `launchSettings.json` sets).
- **IIS / servers:** environment variables (`Section__Key`). `deploy-api.ps1` copies every user-secret of the deploying Windows user into environment variables of the IIS application pool (stored in `applicationHost.config`, readable only by Administrators/SYSTEM); the app pool identity cannot read the developer's user-secrets itself.

| Key | Required | Purpose |
| --- | --- | --- |
| `ConnectionStrings:Default` | yes | SQL Server connection string |
| `Jwt:SigningKey` | yes | HMAC key for access tokens, ≥ 32 bytes |
| `Smtp:UserName`, `Smtp:Password` | for a real SMTP relay | not needed for Mailpit |

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Server=...;Database=VocabularyTrainer;..." --project VocabularyTrainer.Api
dotnet user-secrets list --project VocabularyTrainer.Api
```

The API fails fast at startup with an explicit message when a required secret is missing. Changing `Jwt:SigningKey` invalidates issued access tokens only; clients renew them transparently with their refresh tokens.

**Nullable reference types** are turned **off** for every project via the repo-root [`Directory.Build.props`](./Directory.Build.props) (`<Nullable>disable</Nullable>` plus `NoWarn` for **CS8632** so existing `string?` annotations do not spam “annotation without context”). Flow-analysis warnings (CS8600–CS8618, etc.) are therefore suppressed solution-wide without per-file pragmas.

## Architecture

A vocabulary training app with two clients sharing a layered backend: a **REST API** (`VocabularyTrainer.Api`) and a **WinForms desktop app** (`VocabularyTrainer.WinApp`). The desktop app talks to the API via `VocabularyTrainer.WinApp.ApiClient`.

The API is designed to be client-agnostic: in the future it may be consumed by additional front-ends such as a Telegram bot, a mobile app, or a web SPA. Keep API-facing concerns (DTOs, error contracts, authentication) generic — do not bake in WinApp-specific assumptions.

**Layer dependency order (top → bottom):**

```
WinApp  →  WinApp.ApiClient  →  Api.Contract
Api     →  Api.BusinessLogic →  BusinessLogic  →  DataAccess  →  Domain
Api     →  Infrastructure (SMTP, future Telegram/Viber senders)  →  Domain
                                                        ↑
                                                     Common  (Result/Result<T> + ResultErrorKind)
```

### Projects

- **Domain** — entities (`User`, `UserIdentity`, `RefreshToken`, `VerificationCode`, `Algorithm`, `UserDictionary`, `Word`, `WordTranslation`, `WordWeightBasedParams`, `WordSm2Params`), domain models/DTOs, repository and service interfaces, exceptions. Zero external dependencies.
- **DataAccess** — EF Core (SQL Server) repositories. `VocabularyTrainerDbContext` implements `IVocabularyTrainerDbContext`; Fluent-API configurations live under `Configurations/`. **On the API host**, `AddDbContext` + `IVocabularyTrainerDbContext` are **scoped** (one EF context per HTTP request); repositories are **scoped** and share that context. `WordWeightBasedParamsRepository` / `WordSm2ParamsRepository` open a **nested scope** only on insert-after-unique-race retry so the retry uses a fresh context. The WinApp does **not** call `AddDataAccess` (it uses HTTP repositories). No raw SQL files, no Dapper, no migrations.
- **BusinessLogic** — domain service implementations. Key pieces:
    - `Services/Auth/` — `AuthService`, `VerificationCodeService`, `LoginNormalizer`, `PasswordHasher`, `EmailVerificationCodeSender`, `ExternalAuthProviderRegistry`. See **Authentication** below.
    - `WordService`, `DictionaryService`, `UserService` (server-only: profile + linked identities) — generic CRUD/orchestration services returning `Result`/`Result<T>`. They are the **single source of truth** for input validation, dictionary-ownership checks, text normalisation, and language-code resolution. The `Api.BusinessLogic` layer above only translates `Result` → HTTP, it does not re-validate. The `IDictionaryRepository` (in `WordService`), `ITrainingAlgorithmRegistry`, and `IAlgorithmRepository` (in `DictionaryService`) dependencies are constructor-optional so the WinApp host (where the repository is HTTP-backed and the registry / algorithm lookup are absent) can degrade these services to safe passthroughs and let the server be the authoritative validator.
    - `WordTextNormalizer` — collapses whitespace, casefolds invariant, strips diacritics. Used to compute the per-`(user, dictionary)` deduplication key (`Words.NormalizedText`).
    - `Services/Algorithms/` — pluggable training algorithms behind `IWordTrainingAlgorithm` (`WeightBasedAlgorithm`, `Sm2Algorithm`) and a `TrainingAlgorithmRegistry` that maps an `AlgorithmCode` (string) to an implementation.
    - `TrainingService` — API-side facade implementing `ITrainingService`; resolves the right algorithm for a dictionary and delegates `GetTrainingCandidatesAsync` / `ApplyReviewAsync` to it. Returns `Result<T>` so failures (dictionary not found, unknown algorithm) become proper 4xx responses instead of unhandled exceptions.
    - `WordTrainerService` — WinApp-side stateful session orchestrator (`ITrainingClient` is the abstraction it talks to over HTTP).
- **Api.BusinessLogic** — thin API-facing facade over the domain services. Each `IApi*Service` has the same shape as the underlying `I*Service` and returns the same `Result`/`Result<T>` — there is no separate "API result" type. `IApiTrainingService` is split out from `IApiWordService` so training endpoints live on a separate controller. `IApiAlgorithmService` exposes the algorithm catalog. The layer exists so controllers depend on an API-owned interface (room for future API-only concerns: auth scoping, request enrichment, response shaping) without leaking into `BusinessLogic`. Registered via `AddApiBusinessLogic(connectionString)`, which internally calls `AddVocabularyTrainerForApi` from `BusinessLogic`.
- **Infrastructure** — adapters to external services behind Domain ports. Currently `SmtpEmailSender : IEmailSender` (MailKit, config section `Smtp`; Mailpit on `localhost:1025` for development). Registered by the API via `AddInfrastructure(configuration)`.
- **Api** — ASP.NET Core controllers (`Auth`, `Me`, `Words`, `Training`, `Dictionaries`, `Algorithms`) + a single `MappingProfile` (AutoMapper) mapping `Api.Contract` DTOs ↔ `Domain.Models`. The mapper does not use `ctx.Items` magic — values that come from the route (e.g. `wordId`, `dictionaryId`) are composed into the domain request explicitly by the controller. Controllers depend on `Api.BusinessLogic` services (e.g., `IApiDictionaryService`), **never** on repositories or `BusinessLogic` services directly. `BaseApiController` lives in `Api/Infrastructure/`; its `ResolveFailure(IResult result)` maps `ResultErrorKind` to the appropriate HTTP status and a `ProblemDetails` body.
- **Api.Contract** — HTTP request/response DTOs only; shared by the API and `WinApp.ApiClient`.
- **WinApp.ApiClient** — implements the same `Domain` interfaces as `DataAccess`, but calls the REST API instead of SQL Server. In addition to repositories, it provides `HttpTrainingClient` (implements `ITrainingClient`) and `HttpAlgorithmCatalog` (implements `IAlgorithmCatalog`) for the WinApp. `Auth/` holds the client session: `AccountClient` (`IAccountClient`: login/register/reset/logout, silent restore) and `AuthHeaderHandler` (adds the bearer token, refreshes once on 401, throws `SessionExpiredException` when the session is gone). The refresh token is persisted through `ITokenStore`, which the host supplies. `AccountSettingsClient` (`IAccountSettingsClient`) covers `api/me`: display name, password change (keeps this device, signs out the others), linked sign-in methods, sign-out everywhere.
- **WinApp** — MVP pattern. Startup (`Program.SignIn`) restores the session from `DpapiTokenStore` or shows `LoginForm` (`LoginFormPresenter`); `MainFormPresenter` takes the signed-in user from `IAccountClient.CurrentUser`. Sign-out and session expiry restart the process. `MainFormPresenter` (split across partial classes per tab — `MyWords`, `MyDictionaries`, `AddWord`, `TrainYourself`, `Account`) holds all UI logic; `IMainFormView`/`MainForm` is the passive view. The Account tab is built in code (`MainForm.Account.cs`, auto-sizing layout panels), not in the designer. Every key control gets a hover hint: `MainForm.ToolTips.cs` for designer controls, `_toolTip.SetToolTip` at creation for controls built in code; keep hints in sync with behaviour when changing a control. DI is wired in `Infrastructure/AppStart/DependencyResolver`. User-facing strings live in `Infrastructure/Constants.cs`.
- **Common** — universal class library (future NuGet). Contains `Result`/`Result<T>` wrappers (carrying a `ResultErrorKind` for the failure category — `Validation` / `NotFound` / `Conflict` / `Unauthorized` / `TooManyRequests` / `Unknown`) and shared extensions (e.g., `IsNullOrEmpty`). Host-agnostic; no web concepts. Suitable for WinForms, the API, and any future client.
- **Database** — SQL Server Data Tools `.sqlproj` kept as a **mirror of the production schema**. Schema files: `Users`, `UserIdentities`, `RefreshTokens`, `VerificationCodes`, `Algorithms`, `Dictionaries`, `Words`, `WordTranslations`, `WordWeightBasedParams`, `WordSm2Params`. **EF Core migrations are not used.** Schema changes are made by editing the SQL files in `VocabularyTrainer.Database/`, by hand-writing matching migration scripts in `VocabularyTrainer.Database/Migrations/`, and by running them manually against the database.

## Authentication

One **account** (`Users`) per person, many **identities** (`UserIdentities`: unique `(Provider, ProviderKey)`) — the ways to sign in. Provider codes live in `IdentityProviders` (`email`, `phone`, `telegram`, `google`, `apple`, `facebook`). `ProviderKey` is canonical: lower-cased email, E.164 phone, or the provider's stable user id (never an email for external providers). The password belongs to the account (`Users.PasswordHash`, nullable) and works with any email/phone identity of that account.

Every sign-in path ends in `AuthService.IssueTokensAsync`: the API issues its **own** short-lived JWT access token (`sub` = user id, 15 min, `Jwt` config section) plus an opaque refresh token (60 days, stored only as SHA-256 in `RefreshTokens`). Refresh **rotates** the token; presenting an already-rotated token revokes all of the user's sessions (theft detection). Password reset also revokes all sessions.

One-time codes (`VerificationCodes`, SHA-256 bound to channel/target/purpose) confirm ownership of an email before registration, password reset, or linking. Limits (TTL, resend cooldown, per-hour cap, max attempts) come from the `Auth` config section. The credential endpoints (codes, register, login, external, password reset) are also rate-limited per IP; refresh and logout are not, because server-side clients and NAT-ed users share one IP. Codes and refresh tokens are purged one day after they expire by `AuthCleanupHostedService` (API host, every 6 h) via `IAuthMaintenanceService`.

**API rules.** All endpoints require a bearer token (global fallback policy); `AuthController` is `[AllowAnonymous]`. Controllers take the user id **only** from `BaseApiController.CurrentUserId` (the `sub` claim) — never from the request. Contracts carry no `UserId`. Domain services keep their `int userId` parameters.

**Extending.**
- New code delivery channel (Telegram/Viber for phone numbers, SMS): implement `IVerificationCodeSender` with `Channel = IdentityProviders.Phone` and register it. Phone normalization lives in `LoginNormalizer`.
- New sign-in provider (Telegram, Google, Apple, Facebook): implement `IExternalAuthProvider` (validate the provider credential → `ExternalIdentity`) and register it; `POST api/auth/external/{provider}` then signs in or creates the account.
- New client: send a distinct `ClientInfo.ClientType` (e.g. `"Telegram"`); store the refresh token securely on that platform.

Accounts are never merged automatically by matching email. Linking an identity to an existing account goes through `api/me/identities` (with a code) and fails with 409 if the identity belongs to another account.

## Core Domain Concept: Pluggable Training Algorithms

Each `Dictionary` references an `Algorithm` (FK `AlgorithmId → Algorithms.ID`) that selects a training algorithm at runtime via `ITrainingAlgorithmRegistry`. The algorithm-specific data lives in **satellite tables** (1:1 with `Words`), not on `Words` itself, so a word can carry parameters for multiple algorithms in parallel and switching algorithms doesn't destroy prior progress.

The `Algorithms` table is a small lookup of `(ID, Code)` pairs. The string `Code` (e.g. `"WeightBased"`, `"Sm2"`) is the stable identifier exposed to all upper layers (Domain DTOs, API contracts, WinApp). The integer `Id` only exists inside `DataAccess`/SQL for proper normalisation. On the API host, **`DictionaryService`** resolves `Code → Id` for writes (via `IAlgorithmRepository.GetIdByCodeAsync` and optional `ITrainingAlgorithmRegistry` when there is no DB lookup), passes the FK into `IDictionaryRepository`, and **`DictionaryRepository`** persists it; reads still map `Id → Code` via the `Algorithm` navigation property. The lookup table is **auto-seeded at API startup** by `AlgorithmsSeeder` (which iterates `ITrainingAlgorithmRegistry.All` and inserts any missing code), so adding a new algorithm only requires adding a new `IWordTrainingAlgorithm` and registering it in DI — no manual SQL needed.

Available algorithms:

- **`WeightBased`** (default, free). `WordWeightBasedParams.Weight` (0–`WeightBasedAlgorithm.MaxWeight = 10`) represents how much attention a word needs: **higher weight ⇒ less known ⇒ shown more often**. Candidates are sampled from the **full pool** (not a top-N batch) using Efraimidis–Spirakis weighted reservoir sampling: each card is keyed by `key = -ln(U)/(weight+1)` and the `limit` smallest keys are picked. New cards (no `WordWeightBasedParams` row yet) are treated as `NewCardWeight = MaxWeight/2 = 5` so they surface promptly without dominating an entire session and never starve. Review grades adjust weight: `Again`/`Hard` → +1 (capped at `MaxWeight`), `Good` → -1, `Easy` → -2 (floored at 0). Supported grades: `Again`, `Good`.
- **`Sm2`** (free) — classic SuperMemo-2 spaced-repetition algorithm. `WordSm2Params` stores `Repetitions`, `IntervalDays`, `EaseFactor` (default 2.50), `Lapses`, `LastReviewedAt`, `NextDueAt`. Candidates are words whose `NextDueAt <= UTCNOW` (or never reviewed). All EF/interval math is done in `decimal` to avoid float drift. Supported grades: `Again`, `Hard`, `Good`, `Easy`.

The satellite repositories (`IWordWeightBasedParamsRepository`, `IWordSm2ParamsRepository`) only return the algorithm-specific data (id+weight, ordered ids); the algorithms then hydrate the chosen `WordDto`s via `IWordRepository.GetByIdsAsync`. Both satellites use a concurrency-safe upsert (try `ExecuteUpdateAsync`, fall back to insert; on a unique-key race a second `ExecuteUpdateAsync` settles the value) so concurrent reviews of the same card cannot lose updates or surface a 500.

### Training scope: single dictionary vs. all dictionaries

At the service / API / WinApp boundaries the "which dictionaries to pull from" choice is expressed as a `DictionaryScope` value type (`Domain/Models/DictionaryScope.cs`) — never as a nullable `int? dictionaryId`. The two cases are named (`DictionaryScope.All` vs `DictionaryScope.Single(id)`) so callers can't accidentally mean "all" when they meant "unset".

`DictionaryScope` implements `IParsable<T>`, so ASP.NET Core binds it directly from the query string with no custom binder: the wire format is `?scope=all` or `?scope=42`. `HttpTrainingClient` builds the same URL via `scope.ToString()`. When scope is omitted on the API the default is `DictionaryScope.All` (matches the WinApp's "All dictionaries" combo-box default).

Algorithm and repository layers stay on `int? dictionaryId` (their natural shape — `null` reaches a SQL `WHERE` predicate); only `TrainingService.GetCandidatesAsync` knows about scope. For `DictionaryScope.All` it asks each user dictionary's algorithm for `limit` cards (sequentially — they share the request-scoped DbContext) and round-robin merges the results, so the user sees a balanced mix from every dictionary without any one dominating the batch.

When adding a new algorithm:

1. Create a satellite table `Word<Algo>Params` with `WordId` PK + FK to `Words(ID)` and `ON DELETE CASCADE`.
2. Add a Domain entity, repository interface, and EF Core configuration mirroring the existing pair.
3. Implement `IWordTrainingAlgorithm` under `BusinessLogic/Services/Algorithms/`. Pick a unique `Code` constant; expose its `AlgorithmInfo` (display name, description, cost, supported `ReviewGrade`s).
4. Register both the repository (in `DataAccess.DependencyInjection`) and the algorithm (in `BusinessLogic.DependencyInjection.AddTrainingAlgorithms`).
5. The `Algorithms` row is created automatically on next API startup. Nothing extra in SQL.

## Database

SQL Server only. The connection string is the `ConnectionStrings:Default` secret (see **Secrets** above) — read it with `dotnet user-secrets list --project VocabularyTrainer.Api` if needed; never put credentials into tracked files or documentation.

Schema is **DB-first**: edit the `.sql` files in `VocabularyTrainer.Database/` to keep the SSDT project in sync with the live schema, and write a corresponding migration script in `VocabularyTrainer.Database/Migrations/<NNN_description>.sql` that can be run against the existing database.

### Key tables and relationships

- `Users` (PK `ID`, `DisplayName`, `PasswordHash` NULL, `CreatedAt`, `IsBlocked`).
- `UserIdentities` (PK `ID`, FK `UserId → Users ON DELETE CASCADE`, unique `(Provider, ProviderKey)`).
- `RefreshTokens` (PK `ID`, FK `UserId → Users ON DELETE CASCADE`, unique `TokenHash binary(32)`, `RevokedAt`, `ReplacedById`).
- `VerificationCodes` (PK `ID`, `Channel`, `Target`, `Purpose`, `CodeHash binary(32)`, `ExpiresAt`, `AttemptCount`, `ConsumedAt`).
- `Algorithms` (PK `ID`, unique `Code`). Lookup table; rows are auto-seeded by the API on startup from the in-process algorithm registry.
- `Dictionaries` (PK `ID`, FK `UserId → Users`, FK `AlgorithmId → Algorithms`, unique `(UserId, Name)`, `LanguageCode`).
- `Words` (PK `ID`, FK `UserId → Users`, FK `DictionaryId → Dictionaries`, `Value`, `NormalizedText`, `LanguageCode`, `DateAdded`, `DateModified`, unique `(UserId, DictionaryId, NormalizedText)`). Words are now **per-user** — there is no global word table and no `UserWords` junction. There is no `Notes` column — explanations and mnemonics live as separate `WordTranslations` rows with the appropriate `Kind`.
- `WordTranslations` (PK `ID`, FK `WordId → Words ON DELETE CASCADE`, `Text`, `Kind` enum: `Translation`/`Explanation`/`Mnemonic`). One word can have multiple translations or hint rows; the first `Translation` is treated as the primary translation in UI.
- `WordWeightBasedParams` (PK/FK `WordId → Words ON DELETE CASCADE`, `Weight`).
- `WordSm2Params` (PK/FK `WordId → Words ON DELETE CASCADE`, SM-2 fields).

## Naming Conventions

### Domain: entities vs models

The `Domain` project has two parallel naming spaces:

- `Domain/Entities/` — used by `DataAccess` and `BusinessLogic` (map 1:1 to DB tables, EF Core-aware via Fluent configurations).
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

### Code formatting

- **Comments** — add `//` or XML doc **only** when something is **non-obvious** from the code alone (invariants, wire quirks, concurrency, why an API ignores a parameter). Do **not** restate what the type/method name already says, do not duplicate `AGENTS.md`, and avoid decorative section banners (`// ----- …`). Prefer naming + structure over narration.

- **Do not column-align declarations.** No padding return types, identifiers, parameter lists, or assignments with extra spaces to make them line up vertically. One space between tokens — that's it. Aligned blocks look pretty for one commit and become noise on the next: every rename or signature change forces a whitespace-only re-align that pollutes diffs and reviews, and sustaining it long-term is impossible. Let the formatter do its job.

  ```csharp
  // BAD — vertically aligned signatures
  Task<Result<List<WordDto>>>              GetCandidatesAsync     (int userId, DictionaryScope scope, int limit);
  Task<Result>                             ApplyReviewAsync       (ReviewWordRequest request);
  Task<Result<IReadOnlyList<ReviewGrade>>> GetSupportedGradesAsync(int dictionaryId, int userId);

  // GOOD — single space between tokens
  Task<Result<List<WordDto>>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit);
  Task<Result> ApplyReviewAsync(ReviewWordRequest request);
  Task<Result<IReadOnlyList<ReviewGrade>>> GetSupportedGradesAsync(int dictionaryId, int userId);
  ```

  The same rule applies to fields, properties, local variables, and `using` aliases.

### C# control flow, naming, and spacing (solution-wide)

- **Indentation** — use **tabs** only (no space-based indentation for code). Repo `[.editorconfig](./.editorconfig)` enforces this for `*.cs`.

- **Vertical density** — do not run more than **2–3 non-empty lines** in a row without a **blank line** between logical steps (group variable setup, then a blank line, then control flow, etc.).

- **Ternary operator** — **always three lines**: condition, then `?` arm, then `:` arm (each typically on its own continuation line). When a **method or property `return`s** a ternary, use a **`get`/`return` block** (or a statement body), not a one-liner expression-bodied member, and keep the ternary on those three lines inside braces.

- **`if` / `else if` / `else`** — omit braces **only** when the branch is a **single short line** (e.g. `if (x) return;`). If **any** branch uses a block `{ ... }`, then **all** branches in that `if` / `else` chain must use braces, even when another branch is only one statement.

- **`foreach` / `for` / `while` / `do`** — the body is **always** a braced block `{ ... }`, never a single statement hanging under the loop header.

- **LINQ / complex expressions in `foreach`** — do not put a long chained call directly in `foreach (...)`; assign to a **named local** first (`languageCodes`, `orderedItems`, …), then `foreach` over that. Names must be **meaningful** (`dictionaries`, not `dicts`; `dictionary`, not `dict`).

- **Lambdas** — expression-bodied members and `=>` delegates are fine **only when they fit comfortably on one line**. If the lambda would span multiple lines, use a **named method**, **local function**, or an **explicit loop** instead.

- **Collection literals** — prefer **`[]` / `[ … ]`** (collection expressions) over **`new List<T> { … }`** or **`new List<T>()`** when the target type is satisfied by a span-backed or list-backed construct (e.g. `IReadOnlyList<T>`, `List<T>`, empty `[]`). Reserve `new List<T>(capacity: …)` only when you truly need list semantics (e.g. incremental `Add` in a loop).

### Multi-line calls and logical blocks in C# (solution-wide)

Apply this layout in **any** class or file — presenters, services, repositories, controllers, etc. Reference example: `MainFormPresenter.MyDictionaries`.

- **Blank lines inside non-trivial methods** — separate logical steps: e.g. after an `await` that assigns a result and before the next `if`; after a guard block that `return`s; before the next major statement. Use the same habit inside lambdas and local functions when they grow beyond a few lines.

- **Avoid starting a multi-line argument list on the same line as the opening `(`** when the call does not fit one line. Prefer either a **single-line** call or a **stacked** call:

  ```csharp
  var input = _addDictionaryFormPresenter.ShowModal(
    _view.DialogOwner,
    _view.NeutralCultures,
    algorithms
  );
  ```

  Rules for that shape: each **argument** on its own line, indented **one level past** the line that starts the statement (`var input = …`); the **closing `)` + `;`** alone on the last line, **aligned with the start of that statement** (same column as `var`).

- **Multi-line expressions as arguments** (e.g. a ternary passed into a method) — same idea: break after `(` if needed; place `?` / `:` arms on continuation lines; put the **closing `);`** on its own line **aligned with the start of the call** (the callee).

  ```csharp
  _view.ShowError(result.ErrorKind == ResultErrorKind.Conflict
    ? Constants.DuplicateDictionaryName
    : result.ErrorMessage
  );
  ```

- **Long `new` in a call** — prefer a **named local** (`request`, `options`, etc.) on its own line, then pass it into the method or constructor, instead of inlining a large `new` inside the call.

- **Short guards** — only when the whole `if` fits **one line** (e.g. `if (input is null) return;`). If the controlled statement is on the **next** line, use **braces**.

### Dependency injection

- `DataAccess.AddDataAccess(connectionString)` — registers scoped `VocabularyTrainerDbContext`, scoped `IVocabularyTrainerDbContext` (same instance per request), and **scoped** repositories.
- `BusinessLogic.AddVocabularyTrainerForApi(connectionString)` — for hosts that own the database (the API). Registers `DataAccess`, **scoped** core domain services (`WordService`/`DictionaryService`), **scoped** auth services (`AuthService`, `UserService`, `VerificationCodeService`, email code sender, external-provider registry; the host adds `ITokenService` + `IEmailSender` and binds `AuthOptions`), **scoped** `IWordTrainingAlgorithm` implementations + `ITrainingAlgorithmRegistry`, **scoped** `ITrainingService` and `AlgorithmsSeeder`.
- `BusinessLogic.AddVocabularyTrainerForApiClient()` — for hosts that consume an external API (the WinApp). Registers **singleton** core services + **singleton** `IWordTrainerService` (no EF; avoids scoped dependencies inside a long-lived presenter host). The host must additionally register an `ITrainingClient` and `IAlgorithmCatalog` (the WinApp does this in `WinApp.ApiClient.DependencyInjection`).
- `Api.BusinessLogic.AddApiBusinessLogic(connectionString)` — registers **scoped** `IApi*` services and calls `AddVocabularyTrainerForApi` internally. **This is what the API uses.**
- WinApp wires API-backed repositories via `DependencyResolver` + `ServiceCollectionExtensions`, then calls `AddVocabularyTrainerForApiClient`.

## Error Handling Pattern

**Rule: throw for unexpected errors, return a `Result` for expected failures.**

### What counts as expected vs unexpected

| Situation                            | Pattern                                                                    |
| ------------------------------------ | -------------------------------------------------------------------------- |
| Duplicate name constraint violation  | `Result.Failure(..., ResultErrorKind.Conflict)`                            |
| Entity not found                     | `Result.Failure(..., ResultErrorKind.NotFound)`                            |
| Validation failures                  | `Result.Failure(..., ResultErrorKind.Validation)`                          |
| DB connection refused / SQL timeout  | Throw `DatabaseException`                                                  |
| Unexpected HTTP error from the API   | Throw via `EnsureSuccessStatusCode()`                                      |
| Any other unexpected exception       | Let it propagate (caught by global handler / presenter)                    |

### Result type

There is **one** result type used end-to-end: `Common.Wrappers.Result` / `Result<T>`. It carries:

- `Successful` (bool)
- `ErrorMessage` (string)
- `ErrorKind` (`ResultErrorKind`: `Validation` / `NotFound` / `Conflict` / `Unauthorized` / `TooManyRequests` / `Unknown`)

The same type travels from **domain services** (`BusinessLogic`), through `Api.BusinessLogic`, to the controller. **Repositories do not return `Result`** — they return DTOs/counts/`null` where appropriate and translate duplicate-key SQL or expected HTTP statuses into **domain exceptions** (`DuplicateKeyException`, `EntityNotFoundException`, `DomainValidationException`) so services can map them to `Result`/`Result<T>` with the right `ResultErrorKind`. The controller's `BaseApiController.ResolveFailure(IResult)` maps `ErrorKind` to the appropriate HTTP status code (400 / 404 / 409 / 401 / 429, fallback 500). The WinApp branches on `ErrorKind` to pick a user-facing message. There is no separate `ApiOperationResult` type and no message-text parsing anywhere.

### Repository and service interfaces

- **`IDictionaryRepository` / `IWordRepository` / `IUserRepository`** — persistence operations only (e.g. `Task<int>` rows affected or new id, `Task<UserModel?>` for lookup). `IDictionaryRepository.AddAsync` / `UpdateAsync` take a resolved `algorithmPersistenceId` (HTTP client implementations ignore it for the wire body).
- **Domain services** (`IUserService`, `IDictionaryService`, `IWordService`, …) return `Result`/`Result<T>` and own validation plus mapping from repository outcomes and domain exceptions.

Repositories wrap external calls (SQL, HTTP) in try-catch:

- **DataAccess**: duplicate-key `DbUpdateException` (2601/2627) → throw `DuplicateKeyException`; other DB failures → throw `DatabaseException`.
- **WinApp.ApiClient**: expected HTTP codes → throw the same domain exceptions (or return `0` / `null` where the contract is a raw count/absence); unexpected responses → `EnsureSuccessStatusCode()`.

### API controllers

All controllers inherit `BaseApiController` (lives in `VocabularyTrainer.Api/Infrastructure/`). Controllers depend on `Api.BusinessLogic` services (e.g., `IApiDictionaryService`) and use `IMapper` to translate between `Api.Contract` DTOs and `Domain.Models`.

Controllers call `ResolveFailure(result)` for any unsuccessful `Result`/`Result<T>`:

```csharp
var result = await service.AddAsync(mapper.Map<DomainAddDictRequest>(request));
if (!result.Successful)
    return ResolveFailure(result);

return CreatedAtRoute("GetDictionaryById", new { dictionaryId = result.Value.Id, userId = request.UserId }, ...);
```

There is no per-endpoint try-catch. `GlobalExceptionHandlerMiddleware` in `VocabularyTrainer.Api/Middleware/` catches all unhandled exceptions, logs them, and returns a generic 500 `ProblemDetails` response.

### WinApp presenter

All async presenter actions are wrapped in `ExecuteIfFreeAsync`, which sets an `_isBusy` guard and catches:

- `DatabaseException` → `_view.ShowError(Constants.DatabaseError)`
- `Exception` (catch-all) → `_view.ShowError(string.Format(Constants.UnexpectedError, ex.Message))`

Expected failures (duplicate name, user not found) come back as `Result` from services and are checked inline. Branch on `result.ErrorKind` to pick the right user-facing message:

```csharp
var result = await _dictionaryService.AddAsync(request);
if (!result.Successful)
{
    _view.ShowError(result.ErrorKind == ResultErrorKind.Conflict
        ? Constants.DuplicateDictionaryName
        : result.ErrorMessage);
    return;
}
```

Sign-in goes through `IAccountClient` (`LoginFormPresenter`), which returns the same `Result` with `ErrorKind`. An expired session surfaces as `SessionExpiredException`, which `ExecuteIfFreeAsync` turns into a message plus an app restart.
