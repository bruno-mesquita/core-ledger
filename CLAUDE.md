# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**CoreLedger** — modular banking platform (Phase 1 monolith). Replaced aprendendo-api.

## Commands

```bash
# Start PostgreSQL (required for API)
docker compose up postgres -d

# Run dev server
dotnet run --project src/CoreLedger.API/CoreLedger.API.csproj

# Build
dotnet build

# Unit tests (no Docker needed)
dotnet test tests/CoreLedger.UnitTests/

# Integration tests (requires Docker)
dotnet test tests/CoreLedger.IntegrationTests/

# All tests
dotnet test

# Single test
dotnet test --filter "FullyQualifiedName~TestName"

# EF Core migrations
dotnet ef migrations add <Name> \
  --project src/CoreLedger.Infrastructure \
  --startup-project src/CoreLedger.API

dotnet ef database update \
  --project src/CoreLedger.Infrastructure \
  --startup-project src/CoreLedger.API

# Docker (full stack)
docker compose up --build
docker compose down
```

## Architecture

Clean Architecture + DDD + CQRS (MediatR) on .NET 10 with Minimal APIs and PostgreSQL.

**Dependency direction:** `SharedKernel` ← `Domain` ← `Application` ← `Infrastructure` ← `API`

```
src/
  CoreLedger.SharedKernel/   # Entity, AggregateRoot, Result<T>, IDomainEvent — zero deps
  CoreLedger.Domain/         # Aggregates: User, Account, Transaction, LedgerEntry + repository interfaces
  CoreLedger.Application/    # Commands/Queries/Handlers (MediatR), DTOs, interfaces (IPasswordHasher, ITokenService, IUnitOfWork)
  CoreLedger.Infrastructure/ # EF Core + Npgsql, repositories, PasswordHasher, TokenService
  CoreLedger.API/            # Minimal API endpoints, Program.cs, Serilog

tests/
  CoreLedger.UnitTests/       # Domain + handler tests (NSubstitute, no DB)
  CoreLedger.IntegrationTests/ # Full endpoint tests (Testcontainers PostgreSQL)
```

## Key Design Decisions

- **MediatR 14 CQRS**: every use case is a Command or Query with a Handler. No service classes.
- **Result<T>**: handlers return `Result<T>`. Never throw business exceptions.
- **ValidationBehavior**: FluentValidation runs as MediatR pipeline. Invalid commands → `ValidationException` → 400.
- **MediatR 14 open behaviors**: registered with `cfg.AddOpenBehavior(typeof(...<,>))`.
- **Idempotency**: transfers check `IdempotencyKey` unique index before executing.
- **Refresh token rotation**: old token revoked, new one issued on each refresh.
- **RowVersion removed**: `IsRowVersion()` is SQL Server-only; removed from Account config for PostgreSQL.
- **Primary constructor injection** throughout.

## Domain Aggregates

- **User**: owns RefreshTokens, Role (Customer/Admin)
- **Account**: Debit/Credit/Close returning Result, Status enum
- **Transaction**: Debit/Credit, TransactionKind (TED/Deposit/Withdrawal)
- **LedgerEntry**: double-entry bookkeeping per transaction

## Default Credentials (dev/test)

- Admin: `admin@coreledger.com` / `Admin@123`
- BCrypt hash (workFactor 11): `$2a$11$UtSPv8lsxLhuuaxY3Ly3puAAwLkEFVTARmWyDDGzSl/nqa5SSDyOy`
- Seeded in `UserConfiguration.HasData`

## Integration Test Pattern

`IntegrationTestBase` implements `IAsyncLifetime`. Each test class gets its own `PostgreSqlContainer` (Testcontainers). `CustomWebApplicationFactory` replaces DbContext with the container connection. `db.Database.EnsureCreated()` applies schema.

## SOLID Principles (enforced)

- **SRP**: each Handler has one reason to change. Repositories do data access only.
- **OCP**: new use case → new Command + Handler. Never modify existing handlers.
- **LSP**: all repository implementations fully substitutable (NSubstitute proves this in unit tests).
- **ISP**: `IPasswordHasher`, `ITokenService`, `IUnitOfWork` are single-purpose.
- **DIP**: Application depends on interfaces only. Infrastructure implements them. Wired in DI only.

**Rule:** `new ConcreteService()` outside DI registration is forbidden.
