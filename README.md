# CQRS-Domain-Driven-Design-Practice

A .NET 10 learning project exploring **tactical Domain-Driven Design** and **CQRS**
through an Order domain. Clean architecture in 4 layers, built session-by-session
with a mentor. Modeling only — no tests.

## Architecture

| Layer | Responsibility | Depends on |
|---|---|---|
| Domain | Aggregates, entities, value objects, domain events, invariants | — |
| Application | Use cases, query ports, `IUnitOfWork` | Domain |
| Infrastructure | EF Core write side, Dapper read side, migrations | Application, Domain |
| Presentation | Minimal APIs | Application, Infrastructure |

CQRS separation: **write = EF Core** (repositories + unit of work), **read = Dapper**
(projections). Query ports and read models live in Application; Dapper adapters live
in Infrastructure.

## Tech Stack

- .NET 10, minimal APIs
- EF Core 10 (SQL Server) — write side
- Dapper + Microsoft.Data.SqlClient — read side (in progress)
- MediatR, FluentValidation — planned (Application/CQRS layer)
- Solution: `cqrs-pratice.slnx`

## Project Structure

```
cqrs-pratice/
├── Domain/                                    # no dependencies
│   ├── Common/
│   │   ├── Abstractions/   IAggregateRoot, IDomainEvent
│   │   ├── BaseClasses/    AggregateRoot, Entity, DomainEvent, ValueObject
│   │   └── Exceptions/     DomainException
│   └── Aggregates/
│       └── OrderAggregate/
│           ├── Entities/         Order, OrderItem
│           ├── Enums/            OrderStatus
│           ├── Events/           OrderItemAdded, OrderItemRemoved, OrderConfirmed, OrderCancelled
│           ├── Repositories/     IOrderRepository
│           └── ValueObjects/     OrderId, OrderItemId, CustomerId, ProductId, Money, Address
├── Application/                              # → Domain
│   ├── Common/Interfaces/        IUnitOfWork
│   └── DependencyInjection.cs    (stub — MediatR/validation pending)
├── Infrastructure/                           # → Application, Domain
│   ├── DependencyInjection.cs    (stub — wiring pending)
│   └── Persistence/
│       ├── Write/
│       │   ├── ApplicationDbContext.cs
│       │   ├── DesignTimeDbContextFactory.cs
│       │   ├── Configurations/   OrderConfigurations, OrderItemConfigurations
│       │   ├── Repositories/     RepositoryBase<T>, EFOrderRepository, EFUnitOfWork
│       │   └── Migrations/       20260817092451_init
│       └── Read/                 (planned — DapperOrderQueryService)
└── Presentation/                             # → Application, Infrastructure
    ├── Program.cs                (OpenAPI + /health only)
    └── appsettings*.json
```

## Domain Model (Order aggregate)

- Lifecycle: `PENDING → CONFIRMED → CANCELLED`, transitions raise domain events
  (`OrderItemAdded`, `OrderItemRemoved`, `OrderConfirmed`, `OrderCancelled`)
- Value objects: `OrderId`, `OrderItemId`, `CustomerId`, `ProductId`, `Money`, `Address`
- Invariants: `MaxQuantityPerLine = 100`, money validation, guarded status transitions
- `IOrderRepository` (Domain port): `GetByIdAsync`, `ExistsAsync`, `Add`, `Remove`

## Persistence (Write / Read split)

- `Persistence/Write/` — `ApplicationDbContext`, configurations (complex types for
  Money/Address, `rowversion`, status-as-string), `RepositoryBase<T>`,
  `EFOrderRepository`, `EFUnitOfWork` (wrapper over the scoped DbContext)
- `Persistence/Read/` — Dapper query adapters (planned: `DapperOrderQueryService`
  behind `Application/Orders/Queries/IOrderQueryService`)

## Database & Migrations

Startup project is `Infrastructure` (the design-time factory resolves the connection
string from Presentation's appsettings via a relative path):

```
dotnet ef migrations add <Name> --project Infrastructure --startup-project Infrastructure
dotnet ef database update      --project Infrastructure --startup-project Infrastructure
```

Current migration: `init` (20260817092451). Database not yet applied.

## Build & Run

```
dotnet build cqrs-pratice.slnx
dotnet run --project Presentation    # /health endpoint
```

## Current Status

- [x] Domain: Order aggregate, value objects, domain events, `IOrderRepository`
- [x] EF write side: DbContext, configurations, migration `init`, `EFOrderRepository`, `EFUnitOfWork`
- [~] Read/write split in `Persistence/` (Write done, Read pending)
- [ ] Application read contracts (`IOrderQueryService`, read models)
- [ ] Dapper implementation in `Persistence/Read/`
- [ ] DI wiring (`AddInfrastructure(connectionString)`), Program.cs registration + order endpoints
- [ ] Application/CQRS layer (MediatR, FluentValidation, command handlers)

## Known Issues (current)

- Build is broken mid-restructure: `RepositoryBase.cs` has a bad
  `using Microsoft.EntityFrameworkCore.Write;` and the migration files still use
  stale namespaces (`Infrastructure.Persistence.Migrations`). Repair queue defined.
- DI and Program.cs not wired yet — `DependencyInjection.cs` is a stub.
