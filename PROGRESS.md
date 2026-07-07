# HelpDesk Backend — Progress

> Прогресс разработки по этапам. Обновляется после каждого завершённого этапа.
> Процесс: маленькие этапы, после каждого — объяснение + архитектурные решения + вопросы с собеседования + ожидание подтверждения.
> Target framework: **net9.0** (в ТЗ был .NET 8; установлен SDK 9). Репозиторий — только backend.

## Легенда
- ✅ done · 🔄 in progress · ⬜ not started

## Roadmap

| # | Этап | Статус |
|---|------|--------|
| 1 | Скаффолд решения (4 проекта Clean Architecture) | ✅ |
| 2 | Доменная модель + правила переходов статусов + unit-тесты | ✅ |
| 3 | EF Core Infrastructure (DbContext, конфигурации) + первая миграция | ✅ |
| 4 | Repository + Unit of Work | ✅ |
| 5 | CQRS-скелет (MediatR, Mapster, FluentValidation + ValidationBehavior) | ✅ |
| 6 | Аутентификация (хеширование паролей, JWT) + тесты | ✅ |
| 7 | Тикеты (CQRS) + правила доступа + тесты валидации | ✅ |
| 8 | Админ-операции (статус/приоритет/назначение) | ✅ |
| 9 | Комментарии | ✅ |
| 10 | MVC-контроллеры + обработка ошибок + Serilog | ✅ |
| 11 | SignalR-уведомления | ✅ |
| 12 | Seeding + миграции | ✅ |
| 13 | NSwag + TS-клиент + финализация README | ✅ |

## Журнал этапов

### Этап 1 — Скаффолд ✅
- Solution `HelpDesk.sln` + 4 проекта: Domain, Application, Infrastructure, Api.
- Направление зависимостей: `Api → Application/Infrastructure`, `Infrastructure → Application → Domain`, `Domain` без зависимостей.
- `Directory.Build.props` с общими настройками (net9.0, nullable, implicit usings).

### Этап 13 — NSwag + TS-клиент + README ✅
- Пакет `NSwag.AspNetCore` 14.2.0; `AddOpenApiDocument` с JWT-security (Bearer) + `AspNetCoreOperationSecurityScopeProcessor`.
- В Development: `UseOpenApi` (`/swagger/v1/swagger.json`) + `UseSwaggerUi` (`/swagger`). **Проверено: 200 на документе (NSwag v14, все пути), UI 302→index.**
- `nswag.json` в корне: генерация OpenAPI из проекта (`aspNetCoreToOpenApi`) → Fetch TS-клиент (`clients/helpdesk-client.ts`).
- README финализирован: стек, запуск, учётки, конфиг, API-таблица, миграции, генерация TS-клиента, тесты, roadmap ✅.

### Этап 12 — Seeding + миграции ✅
- `DbSeeder` (`Infrastructure/Persistence`): идемпотентно (bail-out если есть пользователи) сидит 1 Admin + 3 пользователей + 4 тикета (разные статусы/приоритеты, статусы через доменный `ChangeStatus`) + 4 комментария. Пароли хешируются.
- `Program`: на старте `context.Database.MigrateAsync()` + `DbSeeder.SeedAsync` в scope → свежий клон запускается без ручных шагов.
- Учётки: `admin@helpdesk.local` / `Admin123!`, `{alice,bob,carol}@helpdesk.local` / `Password123!`.
- **E2E проверено на живом сервере (localhost SQL)**: миграции применились, сид создан; admin-логин→JWT; `/api/tickets/all`→4 тикета; `/api/users`→4 юзера; non-admin→403 на `/all`; создание тикета→201 (автор из токена).

### Этап 11 — SignalR ✅
- `ITicketNotifier` (`Abstractions/Notifications`) — контракт уведомлений в Application (транспорт скрыт).
- `TicketsHub` (`Api/Realtime`, `[Authorize]`) — подписка по группам на тикет (`JoinTicket`/`LeaveTicket`); `TicketNotifier` шлёт в группу через `IHubContext`.
- События: `TicketStatusChanged`, `TicketAssigned`, `CommentAdded` — вызываются из соответствующих хендлеров после сохранения.
- `Program`: `AddSignalR`, регистрация нотифаера, `MapHub("/hubs/tickets")` c auth; JWT читается из query `access_token` для WebSocket.

### Этап 10 — MVC-контроллеры ✅
- Пакеты Api: `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.9, `Serilog.AspNetCore` 9.0.0.
- `Program.cs`: Serilog (из конфига), `AddControllers` + `MapControllers`, `AddApplication`/`AddInfrastructure`, `IHttpContextAccessor` + `ICurrentUser`→`CurrentUser`, JWT Bearer (валидация issuer/audience/key/lifetime, `MapInboundClaims=false`, `ClockSkew=0`), политика `Admin`.
- `CurrentUser` (`Api/Authentication`) читает `sub`/роль из `HttpContext`.
- `ExceptionHandlingMiddleware` (`IMiddleware`): исключения → RFC7807 `ProblemDetails` (Validation→400, NotFound→404, Conflict/Domain→409, Forbidden→403, Unauthorized→401, прочее→500 с логом).
- Контроллеры (`Api/Controllers`), базовый `ApiControllerBase` (`[ApiController]` + ленивый `ISender`): `AuthController` (`api/auth`, anon), `TicketsController` (`api/tickets`, +admin на all/status/priority/assign), `CommentsController` (`api/comments`, +admin delete), `UsersController` (`api/users`, admin). Политики через `Authorization/Policies.Admin`.
- Coarse-grained роль — декларативно атрибутами `[Authorize]`; fine-grained ownership — в хендлерах.
- Экшены тонкие: транслируют HTTP в MediatR-запросы; тела «id + payload» — вложенные record'ы в контроллере.
- Культура сообщений FluentValidation зафиксирована на `en`.
- **Изначально сделан на Minimal API, переведён на классические MVC-контроллеры** (маршруты и HTTP-коды 1:1).
- **Smoke-тест пройден**: 401 на защищённом, 400 `ValidationProblemDetails` на невалидном логине.

### Этап 8 — Админ-операции ✅
- `ChangeTicketStatus` (домен проверяет переход → `InvalidStatusTransitionException`), `ChangeTicketPriority`, `AssignTicket` (проверка существования исполнителя), `GetAllTickets` (дашборд). Все + валидаторы.
- Доступ Admin — coarse-grained на уровне endpoints (этап 10), поэтому admin-хендлеры не тянут `ICurrentUser`.

### Этап 9 — Комментарии ✅
- `AddComment` (доступ как у деталей: владелец/админ), `GetTicketComments`, `DeleteComment` (admin, `IRequest` без результата).
- `ICommentRepository.GetByIdWithUserAsync` — перечитывание с автором для DTO.

### Этап 7 — Тикеты (CQRS) ✅
- `ICurrentUser` (`Abstractions/Authentication`) — личность вызывающего (Id/IsAdmin/IsAuthenticated); реализация в Api на этапе 10.
- App-исключения: `NotFoundException` (404), `ForbiddenAccessException` (403).
- DTO: `TicketDto` (список), `TicketDetailsDto` (+ комментарии), `CommentDto` (`Application/Comments`). Mapster-конфиг `TicketMappingConfig` (`IRegister`): flatten имён автора/исполнителя (null-safe).
- Фичи (`Application/Tickets`): `CreateTicket` (+валидатор), `GetMyTickets`, `GetTicketById` (правило доступа: владелец или админ), `UpdateTicket` (только автор; домен запрещает после назначения).
- Автор тикета берётся из `ICurrentUser`, не из payload. Create/Update перечитывают тикет с `Include` для DTO с именами.
- `TicketRepository.GetByCreatorAsync` дополнен `Include(CreatedBy/AssignedTo)`.
- Тесты: валидатор создания тикета (5) + доступ в `GetTicketById` (3, NotFound/Forbidden/Admin). **Всего 38 зелёных.**
- Design-time connection обновлён на `Server=localhost` (синхронно с appsettings).

### Этап 6 — Аутентификация ✅
- Абстракции в Application (`Abstractions/Authentication`): `IPasswordHasher`, `IJwtTokenGenerator`.
- Реализации в Infrastructure (`Authentication`): `Pbkdf2PasswordHasher` (PBKDF2/HMAC-SHA256, соль 128 бит, 100k итераций, `FixedTimeEquals`, самоописывающийся формат `iterations;salt;hash`); `JwtTokenGenerator` (HS256, claims sub/email/name/role+jti); `JwtSettings` (секция `Jwt`).
- Фичи (`Application/Authentication`): `RegisterCommand`/`LoginCommand` + валидаторы + хендлеры; `AuthResponse` (token + профиль).
- App-исключения: `ConflictException` (409, дубль email), `UnauthorizedException` (401, неверные креды; единое сообщение — без user enumeration).
- Register: нормализация email (trim+lower), самрегистрация всегда роль `User`, пароль хешируется.
- Пакеты Infrastructure: `System.IdentityModel.Tokens.Jwt` 8.3.0, `Microsoft.Extensions.Options.ConfigurationExtensions`/`Configuration.Abstractions` 9.0.9.
- **`AddInfrastructure` теперь принимает `IConfiguration`** (нужны connection string + секция Jwt); хешер/генератор — Singleton, `Configure<JwtSettings>`.
- Секция `Jwt` в `appsettings.json` (dev-ключ; в проде — secrets/env).
- Тесты (NSubstitute добавлен): хешер (5), генератор JWT (2), Register-хендлер (2), Login-хендлер (3). **Всего 30 тестов зелёные.**

### Этап 5 — CQRS-скелет ✅
- Пакеты в Application: `MediatR` 12.4.1 (свободная лицензия; 13+ коммерческий), `FluentValidation` + `...DependencyInjectionExtensions` 11.11.0, `Mapster` 7.4.0 + `Mapster.DependencyInjection` 1.0.1.
- `AddApplication` (`DependencyInjection.cs`): регистрация MediatR (скан сборки), валидаторов из сборки, `ValidationBehavior` в pipeline, Mapster (`TypeAdapterConfig` + `IMapper`/`ServiceMapper`).
- `ValidationBehavior<TRequest,TResponse>` (`Common/Behaviors`) — валидация как первый шаг pipeline; при ошибках кидает кастомный `ValidationException`.
- `ValidationException` (`Common/Exceptions`) — ошибки сгруппированы по свойству (`IReadOnlyDictionary`), отвязка API от FluentValidation-типов.
- Сквозной пример (реальная фича `/api/users`): `UserDto` (без `PasswordHash`) + `GetUsersQuery` + `GetUsersQueryHandler` с Mapster-маппингом.
- Конвенция: вертикальные срезы по фиче (`Application/<Aggregate>/<Feature>/`).
- Build чистый, 18 тестов зелёные.

### Этап 4 — Repository + Unit of Work ✅
- Абстракции в Application (`Abstractions/Persistence`): `IUnitOfWork`, `IUserRepository`, `ITicketRepository`, `ICommentRepository`. Методы — только под реальные фичи ТЗ, без спекулятивных.
- EF-реализации в Infrastructure (`Persistence/Repositories` + `UnitOfWork`): query-методы `AsNoTracking` (+ нужные `Include`), мутационные загрузки трекаются.
- `UnitOfWork` оборачивает `DbContext.SaveChangesAsync`; общий scoped-контекст с репозиториями → один commit на бизнес-операцию.
- Регистрация в `AddInfrastructure` (`AddScoped` для UoW и трёх репозиториев).
- Репозитории возвращают `IReadOnlyList`/сущности, не `IQueryable` — граница запросов остаётся в Infrastructure, Application не зависит от EF.
- Solution собирается: 0 warnings / 0 errors.

### Этап 3 — EF Core Infrastructure ✅
- Пакеты `Microsoft.EntityFrameworkCore.SqlServer` и `...Design` v**9.0.9** (EF 10 требует net10). Локальный инструмент `dotnet-ef` 9.0.9 (`.config/dotnet-tools.json`).
- `HelpDeskDbContext` (`Persistence/`) с `DbSet`'ами Users/Tickets/Comments; конфигурации применяются из сборки.
- `IEntityTypeConfiguration` на каждую сущность (`Persistence/Configurations`): длины строк, `ValueGeneratedNever` для Guid-ключей, уникальный индекс по Email, связи и delete behaviors, `PropertyAccessMode.Field` для приватной коллекции `_comments`.
- Delete behaviors: Ticket→Comments = Cascade; User→Tickets/Comments = Restrict (защита истории + отсутствие multiple cascade paths в SQL Server).
- `HelpDeskDbContextFactory` (`IDesignTimeDbContextFactory`) — миграции без запуска Api; connection можно переопределить `HELPDESK_DESIGN_CONNECTION`.
- `DependencyInjection.AddInfrastructure(connectionString)` — регистрация `DbContext` (будет вызвана в Api на этапе 10).
- Connection string `DefaultConnection` в `appsettings.json` (LocalDB).
- Миграция `InitialCreate` сгенерирована и **применена к LocalDB** (`dotnet ef database update` → Done).
- `dotnet build HelpDesk.sln` — 0 warnings / 0 errors.

### Этап 2 — Доменная модель ✅
- Enum'ы: `TicketStatus`, `TicketPriority`, `UserRole` (`src/HelpDesk.Domain/Enums`).
- Сущности: `User`, `Ticket`, `Comment` (`src/HelpDesk.Domain/Entities`) с инкапсуляцией (private setters) и доменным поведением.
- `Ticket` — агрегат: методы `ChangeStatus`, `ChangePriority`, `Assign`, `UpdateDetails`; флаг `IsEditableByAuthor`.
- Правило переходов `TicketStatusRules` (`Open → InProgress → Resolved → Closed`), единый источник истины.
- Доменные исключения: `DomainException` (база), `InvalidStatusTransitionException`, `TicketAlreadyAssignedException`.
- Тест-проект `tests/HelpDesk.UnitTests` (xUnit), добавлен в solution. **18 тестов зелёные** (переходы статусов + правило редактирования до назначения).
- ID сущностей — `Guid` (генерируются в конструкторе; снижают риск перебора тикетов по ID).
- `dotnet build` и `dotnet test` — без ошибок и предупреждений.