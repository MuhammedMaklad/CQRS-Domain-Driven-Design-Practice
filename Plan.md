# Plan — Full DDD + CQRS Practice Project

## Architecture Decisions

| Decision | Choice |
|----------|--------|
| Goal | Full end-to-end DDD + CQRS |
| CQRS library | MediatR |
| Read (query) side | Dapper projections |
| Domain depth | Order lifecycle (pending → submitted → shipped → completed) |
| Aggregate identity | OrderId = Guid-based value object (domain-owned) |
| API style | Minimal APIs |
| Write side | EF Core (SQL Server) |

## Layer Dependencies

- `Domain` — no dependencies
- `Application` → `Domain`; packages: `MediatR`, `FluentValidation.DependencyInjectionExtensions`
- `Infrastructure` → `Application`, `Domain`; existing EF Core + Dapper + SqlClient packages
- `Presentation` → `Application`, `Infrastructure`

---

## Phase 0 — References & Packages

- `Application` → add project reference to `Domain`; add packages `MediatR` and `FluentValidation.DependencyInjectionExtensions`
- `Infrastructure` → add project reference to `Application`
- `Presentation` → add project references to `Application` and `Infrastructure`

## Phase 1 — Domain Hardening

1. Make `DomainException` `public` (`Domain/Common/Exceptions/DomainException.cs`)
2. Change `OrderId` to `readonly record struct OrderId(Guid value)` with `OrderId.New()` factory; keep `CustomerId`/`ProductId` as `int` VOs
3. Fix `OrderItem`:
   - Correct quantity validation in `UpdateQuantity` / `IncreaseQuantity` / `DecreaseQuantity`
   - Remove unused `orderId` parameter from `OrderItem.Create`
4. Fix `DomainEvent.OccurredOnUtc` — set to `DateTime.UtcNow` at construction
5. Complete `Order` aggregate (`AggregateRoot<OrderId>`):
   - `Create(CustomerId, Address)` generates `OrderId.New()`
   - State machine `pending → submitted → shipped → completed` with guarded transitions
   - `AddItem` / `UpdateItemQuantity` / `RemoveItem` (allowed only while pending) + maintained `ReCalculateTotalPrice()`
   - `Submit()` / `Ship()` / `Complete()`
6. Domain events under `Domain/Aggregates/OrderAggregate/Events/`:
   `OrderCreated`, `OrderItemAdded`, `OrderItemRemoved`, `OrderItemQuantityUpdated`, `OrderSubmitted`, `OrderShipped`, `OrderCompleted`

## Phase 2 — Application Layer

- `Orders/Commands/`: `CreateOrder`, `AddOrderItem`, `UpdateOrderItemQuantity`, `RemoveOrderItem`, `SubmitOrder`, `ShipOrder`, `CompleteOrder` — each with Command + MediatR handler + FluentValidation validator
- `Orders/Queries/`: `GetOrderById`, `GetOrders` (paged) → `OrderReadDto`, `OrderItemReadDto`
- `Orders/Abstractions/`: `IOrderRepository`, `IOrderReadRepository`, `IDomainEventDispatcher`
- `Common/`: `Result` / `Result<T>`, validation `IPipelineBehavior`
- Sample `OrderSubmitted` event handler (simulated notification)

## Phase 3 — Infrastructure

- `OrderConfigurations`: map `OrderId` (Guid) PK via value converter, `Money`/`Address` as complex/owned properties, `OrderItems` one-to-many, status, timestamps
- `ApplicationDbContext`: register configurations; `SaveChangesAsync` dispatches collected domain events via `IDomainEventDispatcher`
- `EFCoreOrderRepository` (write side) + `DapperOrderReadRepository` (read side, raw SQL projections)
- `Infrastructure.DependencyInjection` static class (DbContext, repositories, dispatcher) + EF initial migration

## Phase 4 — Presentation

- Remove weatherforecast template; add `appsettings` connection string
- Minimal API feature endpoints delegating to MediatR
- `AddProblemDetails()`; map `Result` failures and `DomainException` → 400 responses
- Wire all layer DI in `Program.cs`

## Phase 5 — Verification

- `dotnet build` clean
- Apply EF migration, run app, smoke-test full lifecycle over HTTP
- Optional: xUnit project for order state-machine tests (recommended for DDD practice)