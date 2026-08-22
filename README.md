# CQRS-Domain-Driven-Design-Practice

A .NET 10 learning project exploring **tactical Domain-Driven Design** and **CQRS**
through an Order domain. Clean architecture in 4 layers, built session-by-session
with a mentor. Modeling only — no tests.

## Architecture

Clean Architecture with the dependency rule pointing inward — each layer references only
the layers below it:

```
┌─────────────────────────────────────────────────────────────────┐
│ Presentation                                                     │
│ minimal APIs · HTTP · logging                                    │
└───────────────┬─────────────────────────────────┬───────────────┘
                │                                 │
                ▼                                 ▼
┌───────────────────────────────┐   ┌───────────────────────────────┐
│ Application                   │   │ Infrastructure                │
│ MediatR · CQRS                │   │ EF Core (write)               │
│ validation · ports            │   │ Dapper (read)                 │
└───────────────┬───────────────┘   └───────────────┬───────────────┘
                │                                   │
                └────────────────┬──────────────────┘
                                 ▼
                    ┌───────────────────────────────┐
                    │ Domain (no dependencies)      │
                    │ aggregates · VOs · events     │
                    └───────────────────────────────┘
```

Dependencies point **downward** only — `Domain` never references anything above it.

### Layer responsibilities

| Layer          | Responsibility                                                                                         | Key types                                                                                                                                       |
| -------------- | ------------------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Domain         | Pure business model — aggregates, entities, value objects, domain events, invariants, repository ports | `Order`, `OrderItem`, `Money`, `Address`, `OrderId`, `IOrderRepository`, `DomainException`                                                      |
| Application    | Use cases (MediatR), query ports + read models, validation, `IUnitOfWork`, `IDbConnectionFactory`      | `CreateOrderCommand(+Handler)`, `AddOrderItemCommand(+Handler)`, `IOrderQueryService`, `AppException`, `IEventDispatcher`, `EventHandlers/`     |
| Infrastructure | EF Core write side, Dapper read side, outbox + background dispatcher, migrations                       | `EFOrderRepository`, `EFUnitOfWork`, `ApplicationDbContext`, `OutboxProcessor`, `MediatREventDispatcher`, `OrderQueries`, `DbConnectionFactory` |
| Presentation   | Minimal APIs, exception handler, request logging, DI composition root                                  | `OrderEndpoints`, `GlobalExceptionHandler`, `RequestLoggingMiddleware`, `Program.cs`                                                            |

### CQRS: two pipelines

**Command (write) path** — EF Core + aggregates:

```
Endpoint → AddOrderItemCommand
        → ValidationBehavior (FluentValidation, runs before the handler)
        → AddOrderItemCommandHandler
        → IOrderRepository.GetByIdAsync → Order aggregate (AddItem: guard + event)
        → IUnitOfWork.SaveChangesAsync
             collect events from tracked aggregates (+ clear)
             → write OutboxMessage rows → save order + outbox in ONE transaction
```

**Query (read) path** — Dapper + purpose-built projections:

```
Endpoint → GetOrderByIdQuery
        → GetOrderByIdQueryHandler
        → IOrderQueryService (port)
        → OrderQueries (Dapper adapter) → SQL Server
        → OrderReadModel (projection — no domain types leak into responses)
```

CQRS separation: **write = EF Core** (repositories + unit of work), **read = Dapper**
(projections). Command/query handlers, ports, and read models live in Application;
Dapper adapters live in Infrastructure.

### Tactical DDD building blocks

| Pattern                    | Implementation in this repo                                                                                                                                                                                                                                           |
| -------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Aggregate root             | `Order : AggregateRoot<OrderId>` (marker `IAggregateRoot`) — guards `EnsureIsPending()`, recalculates `TotalPrice`, raises domain events                                                                                                                              |
| Entity                     | `OrderItem : Entity<OrderItemId>` — identity-based equality from `Entity<TId>` (`==`, `!=`, `GetHashCode`)                                                                                                                                                            |
| Value objects              | `Money : ValueObject` (structural equality; `Add`/`Subtract`/`Multiply`/`ApplyDiscount`), `Address` sealed record + `Create` factory, id VOs as `readonly record struct` (`OrderId`, `OrderItemId`, `CustomerId`, `ProductId`) with `New()`/`From()`                  |
| Domain events              | `DomainEvent` base (`EventId`, `OccurredOnUtc`) + `OrderItemAdded`, `OrderItemRemoved`, `OrderConfirmed`, `OrderCancelled` — raised in the aggregate, persisted to the outbox by the UnitOfWork (atomic with the save), dispatched via MediatR by a background worker |
| Invariants / guard clauses | `DomainException` thrown from aggregates and VOs, e.g. `OrderItem` quantity 1–`MaxQuantityPerLine` (100), `Money.Create` non-negative amount, status-transition guards                                                                                                |
| Repository ports           | `IOrderRepository` (Domain) → `EFOrderRepository` (Infrastructure); `IOrderQueryService` (Application) → `OrderQueries` (Infrastructure)                                                                                                                              |

## Tech Stack

- .NET 10, minimal APIs
- EF Core 10 (SQL Server) — write side
- Dapper + Microsoft.Data.SqlClient — read side
- MediatR 12.5 — command/query pipeline
- FluentValidation 12.1 — command validation (MediatR pipeline behavior)
- Solution: `cqrs-pratice.slnx`

## Project Structure

```
cqrs-pratice/
├── Domain/                                    # no dependencies
│   ├── Common/
│   │   ├── Abstractions/         IAggregateRoot, IDomainEvent
│   │   ├── BaseClasses/          AggregateRoot, Entity, ValueObject, DomainEvent
│   │   └── Exceptions/           DomainException
│   └── Aggregates/OrderAggregate/
│       ├── Entities/             Order, OrderItem
│       ├── Enums/                OrderStatus
│       ├── Events/               OrderItemAdded, OrderItemRemoved, OrderConfirmed, OrderCancelled
│       ├── Repositories/         IOrderRepository
│       └── ValueObjects/         OrderId, OrderItemId, CustomerId, ProductId, Money, Address
├── Application/                              # → Domain
│   ├── Common/
│   │   ├── Behaviors/            ValidationBehavior
│   │   ├── Events/               DomainEventNotification
│   │   ├── Exceptions/           AppException
│   │   └── Interfaces/           IUnitOfWork, IDbConnectionFactory, IEventDispatcher
│   ├── Orders/
│   │   ├── Commands/CreateOrder/   CreateOrderCommand(+Handler),
│   │   │                           CreateOrderCommandValidations
│   │   ├── Commands/AddOrderItem/  AddOrderItemCommand(+Handler), AddOrderItemValidation
│   │   ├── EventHandlers/          OrderItemAdded/Removed, OrderConfirmed/Cancelled handlers
│   │   ├── Exceptions/             OrderNotFoundException
│   │   └── Queries/                IOrderQueryService, ReadModels, GetOrderById, GetOrders
│   └── DependencyInjection.cs      AddApplication()
├── Infrastructure/                           # → Application, Domain
│   ├── DependencyInjection.cs      AddInfrastructure(IConfiguration) + OutboxProcessor host
│   └── Persistence/
│       ├── Write/                  ApplicationDbContext, DesignTimeDbContextFactory,
│       │                           Configurations (Orders/, OutboxMessageConfigurations),
│       │                           Repositories, Migrations,
│       │                           Dispatching (MediatREventDispatcher),
│       │                           Outbox (OutboxMessage, OutboxProcessor, OutboxSerializer)
│       └── Read/                   DbConnectionFactory, Queries/Orders (OrderQueries,
│                                   OrderRows, OrderReadMapper)
└── Presentation/                             # → Application, Infrastructure
    ├── Program.cs                  AddApplication + AddInfrastructure + OpenAPI +
    │                               ProblemDetails + exception handler + pipeline
    ├── Endpoints/OrderEndpoints.cs MapGroup /api/v1/orders
    ├── Exceptions/GlobalExceptionHandler.cs   DomainException/ValidationException/AppException → 400
    ├── Middlewares/RequestLoggingMiddleware.cs  method/path/status/duration logs
    └── appsettings*.json           connection string (Default)
```

## Domain Model (Order aggregate)

- Lifecycle: `PENDING → CONFIRMED → CANCELLED`, transitions raise domain events
  (`OrderItemAdded`, `OrderItemRemoved`, `OrderConfirmed`, `OrderCancelled`)
- Value objects: `OrderId`, `OrderItemId`, `CustomerId`, `ProductId`, `Money`, `Address`
- Invariants: `MaxQuantityPerLine = 100`, money validation, guarded status transitions
- Factories: `Order.Create(customerId, address)` and `Order.Create(customerId, address, items)`
  (the latter adds items via `AddItem`, preserving duplicate-line merge + validation)
- `OrderItems` exposed as `IReadOnlyCollection` over a private `List` (EF-friendly)
- `IOrderRepository` (Domain port): `GetByIdAsync`, `ExistsAsync`, `Add`, `Remove`

## Validation (FluentValidation)

- `CreateOrderCommandValidator` + `ShippingAddressValidator` + `ItemRequestValidator`
- `AddOrderItemValidation` — order/product/currency non-empty, unit price > 0, quantity 1–100
- Runs **before** the handler via `ValidationBehavior` (MediatR pipeline behavior), which
  throws FluentValidation `ValidationException` on failures
- Rules: non-empty customer/address/currency, price > 0, quantity 1–100
  (`OrderItem.MaxQuantityPerLine`), and at least one item
- → `400 { Message: "Validation Failed", Errors[] }`

## Persistence (Write / Read split)

- **Write (EF Core):** `ApplicationDbContext` + configurations — Money/Address as complex
  types, `rowversion` concurrency, status stored as string. `RepositoryBase<T>`,
  `EFOrderRepository`, `EFUnitOfWork` (scoped-DbContext wrapper, `IUnitOfWork` in Application).
- **Read (Dapper):** `OrderQueries` (implements `IOrderQueryService`) —
  `GetOrderByIdAsync` via `QueryMultipleAsync` (order + items), `GetCustomerOrdersAsync`
  via a single `LEFT JOIN` multi-map round-trip (`splitOn: "Id"`) grouped in memory.
  SQL rows (`OrderRow`, `OrderItemRow`) → `OrderReadMapper.Map`.

## Domain Events & Outbox

Aggregates raise events in-memory (`AggregateRoot.AddDomainEvent`); dispatch is fully
decoupled from the write path:

```
EFUnitOfWork.SaveChangesAsync
  ├─ collect events from tracked aggregates (ChangeTracker.Entries<IAggregateRoot>) + clear
  ├─ map each event → OutboxMessage row (Type = AssemblyQualifiedName, Content = JSON)
  └─ SaveChangesAsync            ← order rows + outbox rows in ONE transaction

OutboxProcessor (BackgroundService, every 2s, batch of 20)
  └─ pending rows → OutboxSerializer.Deserialize → DomainEventNotification<T>
     → IPublisher.Publish → event handlers; success sets ProcessedOnUtc,
       failure stores Error + Attempts and retries next tick
```

- Guarantees: **atomic** event persistence (order + outbox commit together), **crash-safe**
  (unprocessed rows survive restarts), **at-least-once** delivery
- MediatR stays out of the Domain — events are pure records wrapped by
  `DomainEventNotification<T>` before publishing (`MediatREventDispatcher`)
- Current handlers (`Application/Orders/EventHandlers/`) just log each event

Limitations: single-instance only (no claim locking for concurrent workers); retries may
reorder events across ticks; processed rows are never purged; handlers must be idempotent.

## Middleware & Pipeline

Request pipeline (order matters):

```
MapOpenApi (dev) → RequestLoggingMiddleware → UseExceptionHandler → UseHttpsRedirection → endpoints
```

- `RequestLoggingMiddleware` — logs `{Method} {Path} {Status} {Elapsed}ms` for every request
  (in `finally`); on exception it `LogError`s and rethrows. Placed **outside** the exception
  handler so the logged status reflects the real response.
- `GlobalExceptionHandler` (`IExceptionHandler`) —
  - `DomainException` → `400 { Message }`
  - `ValidationException` → `400 { Message, Errors[] }`
  - `AppException` (incl. `OrderNotFoundException`) → `400 { Message: "Invalid Inputs", Details }`
  - anything else → `500 { Message, Inspect }`

## API (Presentation)

| Method | Route                               | Description                                                                                                    |
| ------ | ----------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| GET    | `/api/v1/orders/{id:guid}`          | Order by id (read model)                                                                                       |
| GET    | `/api/v1/orders/customer/{id:guid}` | Orders for a customer                                                                                          |
| POST   | `/api/v1/orders/create`             | Create order → `201 { id }`; invalid input → `400`                                                             |
| POST   | `/item/add`                         | Add item to order → `200 { Message }`; missing order → `400` (`OrderNotFoundException`); invalid input → `400` |
| GET    | `/health`                           | Liveness check                                                                                                 |

POST body:

```json
{
  "customerId": "00000000-0000-0000-0000-000000000001",
  "shippingAddress": {
    "street": "1 Main St",
    "city": "Cairo",
    "postalCode": "12345",
    "country": "EG"
  },
  "itemRequests": [
    {
      "productId": "00000000-0000-0000-0000-000000000101",
      "unitPrice": 10.5,
      "currency": "USD",
      "quantity": 2
    }
  ]
}
```

Missing/empty `itemRequests`, empty address fields, non-positive price, or qty outside
1–100 → `400` with `Errors[]` (validation runs before the handler — no NRE path).

`POST /item/add` body:

```json
{
  "orderId": "cfe9d87a-fb87-4753-9e44-64ccb4787721",
  "productId": "00000000-0000-0000-0000-000000000101",
  "unitPrice": 10.5,
  "currency": "USD",
  "quantity": 2
}
```

An unknown `orderId` → `400` via `OrderNotFoundException`; an unknown `productId` merges
into a new line (no product catalog exists yet).

## Database & Migrations

`dotnet ef` targets `Infrastructure` as project **and** startup (the design-time factory
resolves the connection string from Presentation's appsettings via a relative path):

```
dotnet ef migrations add <Name> --project Infrastructure --startup-project Infrastructure
dotnet ef database update      --project Infrastructure --startup-project Infrastructure
```

Applied migrations: `init` (20260817092451), `outboxPattern` (20260822131425),
`RenameOutboxColumns` (20260822133149).

## Build & Run

```
dotnet build cqrs-pratice.slnx
dotnet run --project Presentation    # http://localhost:5003 (http) / 7100 (https)
```

Verified end-to-end: `POST /api/v1/orders/create` persists via EF, `GET /api/v1/orders/{id}`
projects via Dapper, and raised events dispatch asynchronously through the outbox
(handler logs appear ~≤2s after the response, exactly once per event).
