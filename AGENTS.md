# AGENTS.md — CQRS-Domain-Driven-Design-Practice

## Project Overview
.NET 10 practice project learning **Domain-Driven Design (tactical patterns)** and **CQRS** through an Order domain. Clean architecture with 4 layers: Domain → Application → Infrastructure → Presentation.

## Architecture & Decisions
- **Layers & dependencies:**
  - `Domain` — no dependencies
  - `Application` → `Domain` (MediatR 12.5, FluentValidation 12.1)
  - `Infrastructure` → `Application`, `Domain` (EF Core write side, Dapper read side)
  - `Presentation` → `Application`, `Infrastructure` (minimal APIs)
- **CQRS library:** MediatR (commands/queries + `ValidationBehavior` pipeline)
- **Read (query) side:** Dapper projections (`IOrderQueryService` port + `OrderQueries` adapter)
- **Write side:** EF Core (SQL Server)
- **Aggregate identity:** `OrderId` = Guid-based `readonly record struct` (`New()`/`From()`), domain-owned
- **Order lifecycle:** `PENDING → CONFIRMED → CANCELLED` with domain events at each transition
- **API style:** minimal APIs, `MapGroup` feature endpoints

## Application Conventions (CQRS)
- Feature-first folders under `Application/Orders/` — `Commands/<Feature>/` and `Queries/<Feature>/`
- Commands: `record XxxCommand(...) : IRequest<T>` + handler + FluentValidation validator
- Query ports (`IOrderQueryService`) and read models live in Application; Dapper rows + mapper live in Infrastructure
- Ports in `Application/Common/Interfaces/`: `IUnitOfWork`, `IDbConnectionFactory`
- `ValidationBehavior` (MediatR pipeline) runs validators before handlers, throws `ValidationException` on failures
- DI: `AddApplication()` (MediatR + validators + behavior); `AddInfrastructure(IConfiguration)` (DbContext + port implementations)

## Presentation Conventions
- Pipeline order: `MapOpenApi (dev)` → `RequestLoggingMiddleware` → `UseExceptionHandler` → `UseHttpsRedirection` → endpoints
- `GlobalExceptionHandler`: `DomainException` / `ValidationException` → `400`; anything else → `500`
- Error envelope: `{ Message }` (domain / 500), `{ Message, Errors[] }` (validation)

## Domain Conventions (Tactical DDD)
- Aggregates: `AggregateRoot<TId>` in `Domain/Aggregates/<Aggregate>/`
- Entities: `Entity<TId>`, identity-based equality, private setters, behavior via public methods
- Value Objects: immutable + self-validating — `Money` inherits `ValueObject` (reference example); `Address` is a `record` with factory; id VOs are `readonly record struct`
- Invariants enforced via guard clauses throwing `DomainException` (public, mapped to 400 in Presentation)
- Domain events raised inside the aggregate; dispatch not yet implemented (next step)
- Persistence models never leak into Domain; repositories expose aggregate roots only

## Build & Verify
- `dotnet build cqrs-pratice.slnx` (0 errors; known warnings: NU1903 OpenApi vulnerability, CS8981 `init` migration name)
- EF migrations: `dotnet ef migrations add <Name> --project Infrastructure --startup-project Infrastructure`
- Apply migrations: `dotnet ef database update --project Infrastructure --startup-project Infrastructure`

## Session Workflow (Learning Track)
Guided tactical DDD modules: concept → live refactor → decisions explained. Modeling only (no tests). Code is written collaboratively with the mentor, session by session.