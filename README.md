# CQRS-Domain-Driven-Design-Practice

A .NET 10 learning project exploring **tactical Domain-Driven Design** and **CQRS**
through an Order domain. Clean architecture in 4 layers, built session-by-session
with a mentor. Modeling only — no tests.

## Architecture

| Layer | Responsibility | Depends on |
|---|---|---|
| Domain | Aggregates, entities, value objects, domain events, invariants | — |
| Application | Use cases (MediatR), query ports, read models, `IUnitOfWork`, `IDbConnectionFactory` | Domain |
| Infrastructure | EF Core write side, Dapper read side, migrations | Application, Domain |
| Presentation | Minimal APIs (endpoints) | Application, Infrastructure |

CQRS separation: **write = EF Core** (repositories + unit of work), **read = Dapper**
(projections). Command/query handlers, ports, and read models live in Application;
Dapper adapters live in Infrastructure.

## Tech Stack

- .NET 10, minimal APIs
- EF Core 10 (SQL Server) — write side
- Dapper + Microsoft.Data.SqlClient — read side
- MediatR 12.5 — command/query pipeline
- FluentValidation — planned
- Solution: `cqrs-pratice.slnx`

## Project Structure

```
cqrs-pratice/
├── Domain/                                    # no dependencies
│   └── Aggregates/OrderAggregate/
│       ├── Entities/         Order, OrderItem
│       ├── Enums/            OrderStatus
│       ├── Events/           OrderItemAdded, OrderItemRemoved, OrderConfirmed, OrderCancelled
│       ├── Repositories/     IOrderRepository
│       └── ValueObjects/     OrderId, OrderItemId, CustomerId, ProductId, Money, Address
├── Application/                              # → Domain
│   ├── Common/Interfaces/     IUnitOfWork, IDbConnectionFactory
│   ├── Orders/
│   │   ├── Commands/CreateOrder/   CreateOrderCommand, CreateOrderCommandHandler
│   │   └── Queries/                IOrderQueryService, ReadModels,
│   │                               GetOrderById, GetOrders
│   └── DependencyInjection.cs      AddApplication() — MediatR
├── Infrastructure/                           # → Application, Domain
│   ├── DependencyInjection.cs      AddInfrastructure(IConfiguration) — DbContext + ports
│   └── Persistence/
│       ├── Write/                  ApplicationDbContext, DesignTimeDbContextFactory,
│       │                           Configurations, Repositories (RepositoryBase,
│       │                           EFOrderRepository, EFUnitOfWork), Migrations (init)
│       └── Read/                   DbConnectionFactory,
│                                   Queries/Orders (OrderQueries, OrderRows, OrderReadMapper)
└── Presentation/                             # → Application, Infrastructure
    ├── Program.cs                  AddApplication + AddInfrastructure + OpenAPI + /health
    ├── Endpoints/OrderEndpoints.cs MapGroup /api/v1/orders
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

## Persistence (Write / Read split)

- **Write (EF Core):** `ApplicationDbContext` + configurations — Money/Address as complex
  types, `rowversion` concurrency, status stored as string. `RepositoryBase<T>`,
  `EFOrderRepository`, `EFUnitOfWork` (scoped-DbContext wrapper, `IUnitOfWork` in Application).
- **Read (Dapper):** `OrderQueries` (implements `IOrderQueryService`) —
  `GetOrderByIdAsync` via `QueryMultipleAsync` (order + items), `GetCustomerOrdersAsync`
  via a single `LEFT JOIN` multi-map round-trip (`splitOn: "Id"`) grouped in memory.
  SQL rows (`OrderRow`, `OrderItemRow`) → `OrderReadMapper.Map`.

## API (Presentation)

| Method | Route | Description |
|---|---|---|
| GET | `/api/v1/orders/{id:guid}` | Order by id (read model) |
| GET | `/api/v1/orders/customer/{id:guid}` | Orders for a customer |
| POST | `/api/v1/orders/create` | Create order → `201 { id }`; `DomainException` → `400` |
| GET | `/health` | Liveness check |

POST body:
```json
{
  "customerId": "00000000-0000-0000-0000-000000000001",
  "shippingAddress": { "street": "1 Main St", "city": "Cairo", "postalCode": "12345", "country": "EG" },
  "itemRequests": [ { "productId": "00000000-0000-0000-0000-000000000101", "unitPrice": 10.5, "currency": "USD", "quantity": 2 } ]
}
```

## Database & Migrations

`dotnet ef` targets `Infrastructure` as project **and** startup (the design-time factory
resolves the connection string from Presentation's appsettings via a relative path):

```
dotnet ef migrations add <Name> --project Infrastructure --startup-project Infrastructure
dotnet ef database update      --project Infrastructure --startup-project Infrastructure
```

Current migration: `init` (20260817092451) — applied.

## Build & Run

```
dotnet build cqrs-pratice.slnx
dotnet run --project Presentation    # http://localhost:5003 (http) / 7100 (https)
```

Verified end-to-end: `POST /api/v1/orders/create` persists via EF, `GET /api/v1/orders/{id}`
projects via Dapper.

## Current Status

- [x] Domain: Order aggregate, value objects, domain events, `IOrderRepository`
- [x] EF write side: DbContext, configurations, migration `init` (applied), repos + UoW
- [x] Application/CQRS: MediatR, `CreateOrder` command, `GetOrderById` / `GetCustomerOrders` queries
- [x] Dapper read side: `OrderQueries`, rows + mapper, `DbConnectionFactory`
- [x] DI wiring + Program.cs + endpoints — verified end-to-end
- [~] FluentValidation — planned
- [ ] Domain event dispatch (events raised in aggregate, not yet handled)
- [ ] Concurrency-aware write workflows (Confirm/Cancel endpoints)

## Known Issues

- `Microsoft.OpenApi` 2.0.0 — NU1903 high-severity vulnerability (bump pending)
- Migration class `init` triggers CS8981 (lowercase type name) — cosmetic
- `Application.Common.interfaces` namespace still lowercase — casing wart
- Domain events raised but not dispatched — intentional, next learning step