# HelpDesk — Ticket Management System (Backend)

Production-style HelpDesk backend built with **ASP.NET 9 Minimal API** and **Clean Architecture**.
Demonstrates CQRS (MediatR), EF Core + SQL Server, JWT auth, SignalR real-time updates and a
NSwag-generated TypeScript client for the (separate) React frontend.

> Target framework is **net9.0** (the original spec asked for .NET 8; we use the installed SDK 9 —
> the public APIs used are identical).

## Tech stack

| Concern            | Technology |
|--------------------|------------|
| Web                | ASP.NET 9 Minimal API |
| Data access        | Entity Framework Core, SQL Server (LocalDB in dev) |
| App layer          | MediatR (CQRS), FluentValidation, Mapster |
| Auth               | JWT Bearer, role-based (User / Admin) |
| Real-time          | SignalR |
| Logging            | Serilog |
| API contract       | NSwag (OpenAPI + TypeScript client) |
| Patterns           | Clean Architecture, Repository + Unit of Work |

## Solution layout

```
HelpDesk.sln
src/
  HelpDesk.Domain          # Entities, enums, domain rules (no external deps)
  HelpDesk.Application      # CQRS handlers, DTOs, validators, abstractions
  HelpDesk.Infrastructure   # EF Core, repositories, JWT, external services
  HelpDesk.Api              # Minimal API endpoints, DI composition root
```

Dependency direction: `Api → Application/Infrastructure → Application → Domain`.
Domain depends on nothing; Infrastructure implements abstractions declared in Application.

## Getting started

Prerequisites: **.NET SDK 9** and a **SQL Server** instance (the default connection string points to
`Server=localhost`; adjust `ConnectionStrings:DefaultConnection` in `src/HelpDesk.Api/appsettings.json`
or override via environment/user-secrets).

```bash
dotnet build
dotnet run --project src/HelpDesk.Api
```

On startup the API **applies pending EF Core migrations and seeds** the database automatically, so a
fresh clone is runnable with no manual steps. In Development, Swagger UI is served at `/swagger`.

### Seeded accounts

| Role  | Email                   | Password       |
|-------|-------------------------|----------------|
| Admin | `admin@helpdesk.local`  | `Admin123!`    |
| User  | `alice@helpdesk.local`  | `Password123!` |
| User  | `bob@helpdesk.local`    | `Password123!` |
| User  | `carol@helpdesk.local`  | `Password123!` |

### Configuration

- `ConnectionStrings:DefaultConnection` — SQL Server connection string.
- `Jwt` — `Issuer`, `Audience`, `Key` (signing secret) and `ExpiryMinutes`. The dev key in
  `appsettings.json` is a placeholder; supply a real secret via user-secrets or environment variables
  in any non-dev environment.

## API surface

Grouped Minimal API endpoints (JWT Bearer required unless noted):

| Group          | Endpoints |
|----------------|-----------|
| `/api/auth`    | `POST /register`, `POST /login` (anonymous) |
| `/api/tickets` | `POST /`, `GET /` (mine), `GET /{id}`, `PUT /{id}`; **admin:** `GET /all`, `PUT /{id}/status`, `PUT /{id}/priority`, `PUT /{id}/assign` |
| `/api/comments`| `POST /`, `GET /?ticketId=…`; **admin:** `DELETE /{id}` |
| `/api/users`   | `GET /` (**admin**) |
| `/hubs/tickets`| SignalR hub — events `TicketStatusChanged`, `TicketAssigned`, `CommentAdded` (JWT via `?access_token=`) |

Errors are returned as RFC 7807 `ProblemDetails` (validation errors as `ValidationProblemDetails`).

## Database & migrations

```bash
# add a migration
dotnet dotnet-ef migrations add <Name> --project src/HelpDesk.Infrastructure --startup-project src/HelpDesk.Infrastructure --output-dir Persistence/Migrations
# apply migrations manually (also done automatically on API startup)
dotnet dotnet-ef database update --project src/HelpDesk.Infrastructure --startup-project src/HelpDesk.Infrastructure
```

`dotnet-ef` is pinned as a local tool (`.config/dotnet-tools.json`); run `dotnet tool restore` first.

## TypeScript client generation (NSwag)

The API exposes an OpenAPI 3 document at `/swagger/v1/swagger.json`. A generator config lives in
`nswag.json` (outputs a Fetch-based client to `clients/helpdesk-client.ts`):

```bash
dotnet tool install -g NSwag.ConsoleCore   # once
nswag run nswag.json
```

## Tests

```bash
dotnet test
```

Unit tests (xUnit + NSubstitute) cover the three areas required by the spec — ticket status
transitions, ticket-creation validation and the authentication service — plus access-rule checks.

## Roadmap — delivered ✅

All 13 backend stages are complete (see `PROGRESS.md` for the detailed log):

1. Solution scaffold
2. Domain model & status-transition rules
3. EF Core infrastructure & first migration
4. Repository pattern & Unit of Work
5. Application CQRS skeleton (MediatR, Mapster, FluentValidation)
6. Authentication (password hashing, JWT)
7. Tickets CQRS & access rules
8. Admin operations (status / priority / assignment)
9. Comments
10. Minimal API endpoints, error handling, Serilog
11. SignalR real-time notifications
12. Seeding & startup migrations
13. NSwag OpenAPI document & TypeScript client
