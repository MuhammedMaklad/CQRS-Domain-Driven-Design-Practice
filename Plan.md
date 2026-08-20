# Plan — Full DDD + CQRS Practice Project

Living status document — `[x]` done · `[~]` partial · `[ ]` next.

## Architecture Decisions

| Decision | Choice |
|----------|--------|
| Goal | Full end-to-end DDD + CQRS |
| CQRS library | MediatR 12.5 |
| Read (query) side | Dapper projections (`IOrderQueryService` port) |
| Domain depth | Order lifecycle `PENDING → CONFIRMED → CANCELLED` |
| Aggregate identity | `OrderId` = Guid-based `readonly record struct` (domain-owned) |
| API style | Minimal APIs |
| Write side | EF Core (SQL Server) |
| Validation | FluentValidation 12.1 via MediatR pipeline behavior |

> Note: `AGENTS.md` / `README.md` still describe an older 4-state lifecycle
> (`pending → submitted → shipped → completed`). The implemented model is
> `PENDING → CONFIRMED → CANCELLED` — reconcile the docs or migrate the aggregate.

## Layer Dependencies

- `Domain` — no dependencies
- `Application` → `Domain`; packages: `MediatR`, `FluentValidation` + `FluentValidation.DependencyInjectionExtensions`
- `Infrastructure` → `Application`, `Domain`; EF Core + Dapper + SqlClient packages
- `Presentation` → `Application`, `Infrastructure`

---

## Phase 0 — References & Packages — [x]

- `Application` → project reference to `Domain`; packages `MediatR` 12.5 + `FluentValidation` 12.1 (+ extensions)
- `Infrastructure` → project reference to `Application`
- `Presentation` → project references to `Application` and `Infrastructure`

## Phase 1 — Domain Hardening — [x]

- [x] `DomainException` is `public` (`Domain/Common/Exceptions/DomainException.cs`)
- [x] `OrderId` `readonly record struct` with `New()` / `From()` factories; `CustomerId` / `ProductId` / `OrderItemId` as Guid `readonly record struct`
- [x] `OrderItem` guards: `MaxQuantityPerLine = 100`, `Create` / `UpdateQuantity` / `IncreaseQuantity` / `DecreaseQuantity` validation
- [x] `DomainEvent.OccurredOnUtc` = `DateTime.UtcNow` at construction
- [x] `Order` aggregate (`AggregateRoot<OrderId>`):
  - `Create(CustomerId, Address)` → `OrderId.New()`; plus `Create(CustomerId, Address, IEnumerable<OrderItem>)` overload (adds via `AddItem`)
  - State machine `PENDING → CONFIRMED → CANCELLED` with guarded transitions (`Confirm`, `Cancel`)
  - `AddItem` (merges duplicate product lines), `RemoveItem`, `ReCalculateTotalPrice`
  - `OrderItems` exposed as `IReadOnlyCollection` over a private `List` (EF-friendly)
- [x] Domain events: `OrderItemAdded`, `OrderItemRemoved`, `OrderConfirmed`, `OrderCancelled`

## Phase 2 — Application Layer — [~]

- [x] `Orders/Commands/CreateOrder/` — `CreateOrderCommand` + handler + `CreateOrderCommandValidations`
- [x] `Orders/Queries/` — `GetOrderById`, `GetCustomerOrders` + read models (`OrderReadModel`, `OrderItemReadModel`)
- [x] `Common/Behaviors/ValidationBehavior` — MediatR pipeline behavior (throws `ValidationException` on failures)
- [ ] `Orders/Commands/`: `AddOrderItem`, `UpdateOrderItemQuantity`, `RemoveOrderItem` (wrap existing aggregate methods)
- [ ] `Orders/Commands/`: `ConfirmOrder`, `CancelOrder` + endpoints
- [ ] `IDomainEventDispatcher` port + sample event handler (`OrderConfirmed`)
- [ ] (optional) `Result` / `Result<T>`

## Phase 3 — Infrastructure — [~]

- [x] `OrderConfigurations` / `OrderItemConfigurations` — Money/Address as complex types, `rowversion` concurrency, status-as-string
- [x] `ApplicationDbContext` + write side: `RepositoryBase<T>`, `EFOrderRepository`, `EFUnitOfWork`
- [x] Dapper read side: `DbConnectionFactory`, `OrderQueries` (multi-query + single `LEFT JOIN` multi-map), `OrderRows`, `OrderReadMapper`
- [x] `AddInfrastructure(IConfiguration)` + migration `init` (20260817092451) — applied
- [ ] Domain event dispatch — `SaveChangesAsync` collects and dispatches aggregate events via `IDomainEventDispatcher`

## Phase 4 — Presentation — [x]

- [x] Minimal API `MapGroup("/api/v1/orders")` — `GET /{id}`, `GET /customer/{id}`, `POST /create`
- [x] `AddProblemDetails()` + `GlobalExceptionHandler` (`DomainException` / `ValidationException` → 400, else 500)
- [x] `RequestLoggingMiddleware` (method/path/status/duration; placed outside the exception handler)
- [x] `Program.cs` wired: `AddApplication`, `AddInfrastructure`, endpoints, connection string

## Phase 5 — Verification — [x]

- [x] `dotnet build cqrs-pratice.slnx` clean (0 errors; NU1903 + CS8981 warnings)
- [x] Migration applied; end-to-end smoke test (`POST /api/v1/orders/create` → `GET /api/v1/orders/{id}`)
- [ ] xUnit tests for the state machine — out of scope per AGENTS.md (modeling only)

---

## Roadmap (next)

1. **Domain event dispatch** — `IDomainEventDispatcher` (Domain port) → dispatcher + `SaveChangesAsync` hookup (Infrastructure) → sample `OrderConfirmed` handler
2. **Remaining write commands** — `AddOrderItem`, `UpdateOrderItemQuantity`, `RemoveOrderItem`, `ConfirmOrder`, `CancelOrder` + endpoints (FluentValidation validators each)
3. **Optional** — `Result`/`Result<T>`; paged `GetOrders` query; `Inventory` aggregate + domain service for `ProductId` availability validation; reconcile lifecycle docs (AGENTS.md/README vs `PENDING → CONFIRMED → CANCELLED`)