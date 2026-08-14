# AGENTS.md — CQRS-Domain-Driven-Design-Practice

## Project Overview
.NET 10 practice project learning **Domain-Driven Design (tactical patterns)** and **CQRS** through an Order domain. Clean architecture with 4 layers: Domain → Application → Infrastructure → Presentation.

## Architecture & Decisions
- **Layers & dependencies:**
  - `Domain` — no dependencies
  - `Application` → `Domain` (MediatR, FluentValidation)
  - `Infrastructure` → `Application`, `Domain` (EF Core write side, Dapper read side)
  - `Presentation` → `Application`, `Infrastructure` (minimal APIs)
- **CQRS library:** MediatR
- **Read (query) side:** Dapper projections
- **Write side:** EF Core (SQL Server)
- **Aggregate identity:** `OrderId` = Guid-based value object, domain-owned
- **Order lifecycle:** `pending → submitted → shipped → completed` with domain events at each transition
- **API style:** minimal APIs

## Domain Conventions (Tactical DDD)
- Aggregates: `AggregateRoot<TId>` in `Domain/Aggregates/<Aggregate>/`
- Entities: `Entity<TId>`, identity-based equality, private setters, behavior via public methods
- Value Objects: inherit `ValueObject`, immutable, self-validating, structural equality (`Money` is the reference example)
- Invariants enforced via guard clauses throwing `DomainException` (public, mapped to 400 in Presentation)
- Domain events raised inside the aggregate, dispatched after commit
- Persistence models never leak into Domain; repositories expose aggregate roots only

## Build & Verify
- `dotnet build` (solution: `cqrs-pratice.slnx`)
- EF migrations: `dotnet ef migrations add <Name> --project Infrastructure --startup-project Presentation`
- Apply migrations: `dotnet ef database update --project Infrastructure --startup-project Presentation`

## Session Workflow (Learning Track)
Guided tactical DDD modules: concept → live refactor → decisions explained. Modeling only (no tests). Code is written collaboratively with the mentor, session by session.