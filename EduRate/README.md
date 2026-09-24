# EduRate — Clean Architecture Refactor

This is the original EduRate ASP.NET Core Web API, refactored into Clean Architecture with
CQRS/MediatR, the Repository Pattern, FluentValidation, centralized exception handling, and
one Hangfire recurring job — with every existing feature, endpoint, and business rule preserved.

## Final Architecture

```
EduRate.sln
├── EduRate.Domain          Entities only (Student, Teacher, Center, Session, Booking, Review,
│                           Message, Notification, StudentFavorite, PromoCode, Subject,
│                           TeacherCenter, CenterImage) + the EducationalStage enum.
│                           No dependencies on any other project or package.
│
├── EduRate.Application     Business logic, organized by feature:
│                           Features/<Auth|Students|Teachers|Centers|Sessions|Bookings|
│                                     Reviews|Payments|PromoCodes|Messages|Notifications|
│                                     Subjects>/{Commands,Queries,Dtos.cs}
│                           Common/Interfaces   - repository interfaces, IUnitOfWork,
│                                                 ICurrentUserService, IIdentityService,
│                                                 IJwtTokenGenerator, INotificationService,
│                                                 IPaymobService
│                           Common/Exceptions   - NotFoundException, BadRequestException,
│                                                 UnauthorizedException, ForbiddenAccessException,
│                                                 ValidationException
│                           Common/Behaviours   - ValidationBehaviour (MediatR pipeline)
│                           Depends only on EduRate.Domain (+ MediatR/FluentValidation/EF Core
│                           abstractions - see "Design notes" below).
│
├── EduRate.Infrastructure  EF Core (AppDbContext, migrations), ASP.NET Identity, JWT issuing,
│                           the 13 repository implementations + UnitOfWork, NotificationService
│                           (SignalR), PaymobService (HttpClient), the SessionReminderJob
│                           (Hangfire), the SignalR ChatHub, and the DbSeeder.
│                           Depends on EduRate.Application (+ Domain transitively).
│
└── EduRate.API             Thin controllers (delegate to MediatR), GlobalExceptionMiddleware,
                            Program.cs composition root, appsettings.
                            Depends on Application + Infrastructure.
```

Dependency direction is exactly the required one: `API → Infrastructure → Application → Domain`,
with `API` also referencing `Application` directly for MediatR request types. `Domain` has zero
outward dependencies.

## CQRS + MediatR

Every meaningful state-changing or read operation from the original controllers became a
Command or Query, each in its own file (`Command`/`Query` + its `Handler`, and a `Validator`
where input validation makes sense) under `Features/<Feature>/{Commands,Queries}`. Controllers
now only build the request object (filling in the caller's identity from JWT claims where the
original controller code did) and call `_mediator.Send(...)`. Business logic that used to live
in controller action bodies (wallet math, booking/session conflict checks, review eligibility,
JWT issuance, etc.) now lives entirely in the handlers.

**Not** turned into CQRS: the ~9 endpoints in `CentersController` that were already unimplemented
stubs in the original app (`SearchCenters`, `GetNearbyCenters`, `GetTopCenters`,
`GetCenterTeachers`, `AddTeacherToCenter`, `ToggleTeacherStatus`, `GetCenterSchedule`,
`VerifyCenter`, `GetCenterStats`, `GetCenterImages`, `AddCenterImage`). Per your instructions
these were left exactly as-is (still returning empty `Ok()`/`NoContent()`), with no invented
logic.

## Repository Pattern + Unit of Work

- `Application/Common/Interfaces/IRepository.cs` — a generic `IRepository<TEntity>` with
  `Query()` (returns `IQueryable<TEntity>`, so handlers keep composing the same
  `Include/Where/Select` projections the original controllers used), `GetByIdAsync`, `Add`,
  `AddRange`, `Update`, `Remove`, `RemoveRange`.
- `Application/Common/Interfaces/IEntityRepositories.cs` — one repository interface per
  aggregate/DbSet (`IStudentRepository`, `ITeacherRepository`, `ICenterRepository`,
  `ISessionRepository`, `IBookingRepository`, `IReviewRepository`, `IMessageRepository`,
  `INotificationRepository`, `IStudentFavoriteRepository`, `IPromoCodeRepository`,
  `ISubjectRepository`, `ITeacherCenterRepository`, `ICenterImageRepository`) — thin markers
  over the generic interface; no bespoke methods were added beyond what handlers actually need,
  to avoid over-engineering.
- `Application/Common/Interfaces/IUnitOfWork.cs` — one `SaveChangesAsync()`, shared by every
  repository in the same DI scope (they all wrap the same scoped `AppDbContext`), so a handler
  that touches several aggregates (e.g. `CreateBookingCommand` debits a `Student` and creates a
  `Booking`) still commits atomically with a single call.
- Implementations live in `EduRate.Infrastructure/Persistence/Repositories/` (`Repository<T>`
  generic base class + 13 one-line concrete classes) and `Persistence/UnitOfWork.cs`.
- **No Application handler references `AppDbContext` or any EF Core type directly** — only the
  repository interfaces and `IUnitOfWork`. (Application still references the
  `Microsoft.EntityFrameworkCore` package, but only for the `IQueryable`/async-LINQ extension
  methods used against `Query()` — see "Design notes".)

## FluentValidation

- `Common/Behaviours/ValidationBehaviour.cs` is registered as an open MediatR pipeline behavior
  (`Application/DependencyInjection.cs`), so every Command/Query with a registered validator is
  validated automatically before its handler runs.
- Validators live next to their Command/Query (e.g. `RegisterCommandValidator`,
  `CreateSessionCommandValidator`, `AddOrUpdateReviewCommandValidator`) and replace the
  `[Required]`/manual `if` checks the original DTOs and controllers used for basic input shape.
- Business-rule checks that depend on database state (wallet balance, booking conflicts,
  ownership, review eligibility, etc.) stay in the handlers as before, throwing
  `BadRequestException`/`NotFoundException`/`ForbiddenAccessException` — these still map to the
  same HTTP status codes the original controllers returned.

## Global Exception Handling

`EduRate.API/Middleware/GlobalExceptionMiddleware.cs` is the first middleware in the pipeline.
It maps:
- `ValidationException` → 400 with `{ message, errors }`
- `BadRequestException` → 400 with `{ message }`
- `NotFoundException` → 404 with `{ message }`
- `UnauthorizedException` → 401 with `{ message }`
- `ForbiddenAccessException` → 403 with `{ message }`
- anything else → 500 with a generic message (logged via `ILogger`)

This replaces the original controllers' inconsistent mix of `BadRequest("string")` and
`BadRequest(new { message = "..." })`. **One deliberate, small contract change**: FluentValidation
failures (simple shape checks that used to be enforced by `[ApiController]`'s automatic
`ModelState` 400) now come back as `{ message, errors: {...} }` instead of a bare string — this
is the one place the response envelope changed; every other error path preserves the original
status code and `{ message }` shape (and often the exact original Arabic/English wording).

## Hangfire — the ONE use case

**Feature:** Session reminders.
**Job:** `EduRate.Infrastructure/BackgroundJobs/SessionReminderJob.cs`
**Why:** The original `SessionReminderService` was a hand-rolled `BackgroundService` with its own
`Task.Delay(15 min)` polling loop that checked for sessions starting in ~2 hours and notified the
teacher + booked students. That's exactly the shape of work Hangfire recurring jobs are for:
time-triggered, not user-request-triggered, and benefits from Hangfire's dashboard, retries, and
not drifting on app restarts. It's registered as a recurring job (`*/15 * * * *`, same 15-minute
cadence as the original loop) in `Program.cs`:

```csharp
RecurringJob.AddOrUpdate<SessionReminderJob>(
    "session-reminder-job",
    job => job.CheckUpcomingSessionsAsync(),
    "*/15 * * * *");
```

Storage is the same SQL Server database (`Hangfire.SqlServer`), so no extra infrastructure is
needed. The dashboard is available at `/hangfire` (defaults to local-requests-only; add a proper
`IDashboardAuthorizationFilter` before exposing it publicly). **No other feature uses Hangfire** —
everything else (bookings, payments, reviews, messaging, wallet operations) stays synchronous,
exactly as before.

## Everything else preserved as-is

- JWT auth, ASP.NET Identity, roles/claims (`ProfileId`, `Role`) — unchanged shape and claims.
- SignalR (`ChatHub`) — moved into `EduRate.Infrastructure/Realtime` (not the API project)
  because `NotificationService`, also in Infrastructure, needs `IHubContext<ChatHub>` and
  Infrastructure can't depend on the API project. `Program.cs` still maps the route
  (`/chathub`) since API references Infrastructure.
- Paymob wallet top-up + webhook, the direct/self-service wallet top-up, reward-point redemption,
  booking/session conflict checks, review eligibility + 15-minute edit window, the
  `CentersController.AddCenterReview` vs `ReviewsController.AddOrUpdateReview` duplication (two
  genuinely different review pathways in the original app) — all preserved exactly.
- The large inline database seeder from `Program.cs` moved verbatim into
  `Infrastructure/Persistence/DbSeeder.cs` (same accounts, same random sample data).
- The default `WeatherForecastController` template scaffold — kept, since it was an existing
  endpoint.
- A small pre-existing bug in `EduRate.DTOs.StudentDTOs` (a duplicate, dead-code nested
  `StudentProfileDto` under a doubled namespace block) was dropped during the move — it was
  unreachable dead code, not a real feature.

## Design notes / minor pragmatic choices

- A handful of original actions returned raw domain entities instead of DTOs (e.g.
  `TeachersController.PostTeacher`/`SearchTeachers`, `CentersController.GetMyProfile`). These are
  preserved exactly (the Command/Query returns the entity type) rather than "fixed" into DTOs, to
  avoid changing those endpoints' JSON contracts.
- `Application` references the `Microsoft.EntityFrameworkCore` package (for `IQueryable` and the
  async LINQ extensions like `.Include/.ToListAsync/.AnyAsync` used against `IRepository<T>.Query()`)
  and `Microsoft.Extensions.Configuration.Abstractions` (for `IConfiguration`, used once in the
  Paymob command). It does **not** reference `EduRate.Infrastructure`, `AppDbContext`, or any
  SQL Server-specific package — the dependency rule (`Application` independent of `Infrastructure`)
  is intact.

## How to run

1. **Prerequisites:** .NET 8 SDK, SQL Server (LocalDB, Express, or full) reachable from the
   connection string in `EduRate.API/appsettings.json`.
2. **Connection string:** update `ConnectionStrings:DefaultConnection` in
   `EduRate.API/appsettings.json` (defaults to a `DESKTOP-S6RR08S` server name copied from the
   original project — change this to your own SQL Server instance).
3. **JWT:** `JWT:Key`/`Issuer`/`Audience`/`DurationInDays` are already filled in with working
   defaults (same as the original project) — change `JWT:Key` for any real deployment.
4. **Paymob:** `Paymob:ApiKey`/`IntegrationIdCard`/`IntegrationIdWallet`/`IframeId` are still
   placeholders (`YOUR_API_KEY_HERE`, etc.), exactly as in the original project — fill in real
   values to exercise the payment-gateway flow.
5. From the solution root:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project EduRate.API
   ```
   On first run, `DbSeeder` applies EF Core migrations (`Database.MigrateAsync()`) and seeds
   sample data automatically — **no separate `dotnet ef database update` step needed**, but you
   do need to create an initial migration first since this refactor moved `AppDbContext` to a new
   project/namespace:
   ```bash
   dotnet tool install --global dotnet-ef   # if you don't have it already
   dotnet ef migrations add InitialCreate --project EduRate.Infrastructure --startup-project EduRate.API
   ```
6. Swagger UI opens automatically in development at `/swagger`; the Hangfire dashboard is at
   `/hangfire`.

## A note on verification

This refactor was produced in an environment without the .NET SDK or network access, so it could
not be compiled or run here. Every file was written by hand against the original source with
careful attention to namespaces, DI wiring, and each endpoint's exact original behavior, and the
whole solution was swept afterward for missing `using`s, unresolved types, mismatched DI
registrations, and duplicate/orphaned classes. Still, please run `dotnet build` after extracting
and fix anything a real compiler catches that this review didn't — most likely candidates for any
remaining issue are a missed `using` or a NuGet package version bump.
