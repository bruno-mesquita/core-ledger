# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run dev server
dotnet run --project src/aprendendo-api.API/aprendendo-api.API.csproj

# Build
dotnet build

# Run tests
dotnet test

# Run single test
dotnet test --filter "FullyQualifiedName~TestName"

# EF Core migrations
dotnet ef migrations add <Name> --project src/aprendendo-api.Infrastructure --startup-project src/aprendendo-api.API
dotnet ef database update --project src/aprendendo-api.Infrastructure --startup-project src/aprendendo-api.API

# Docker
docker build -t aprendendo-api .
docker run -p 8080:8080 aprendendo-api
```

## Architecture

Clean Architecture solution (`aprendendo-api.slnx`) targeting .NET 10 with **Minimal APIs** (no MVC controllers).

**Current state:** User CRUD API with JWT auth, role-based authorization (User/Admin), protected admin route, and full test suite (unit + integration).

**Dependency direction:** `Domain` ← `Application` ← `Infrastructure` ← `API`. Never reversed.

```
src/
  aprendendo-api.Domain/          # Entities, interfaces, enums — zero external deps
  aprendendo-api.Application/     # Services, DTOs, application interfaces — depends on Domain
  aprendendo-api.Infrastructure/  # EF Core, JWT, BCrypt — depends on Domain + Application
  aprendendo-api.API/             # Endpoints, Program.cs — depends on Application + Infrastructure
tests/
  aprendendo-api.Tests/           # xUnit unit + integration tests
```

### Key design choices

- Typed results pattern: `Results<Ok<T>, NotFound>` for endpoint return types
- Records for DTOs
- Primary constructor injection throughout
- SQLite + EF Core 10 (AOT disabled — EF Core is not AOT-compatible)
- JWT Bearer auth via `AddAuthentication().AddJwtBearer()`
- BCrypt for password hashing (`BCrypt.Net-Next`)

### Default credentials (dev/test)

- Admin: `admin@example.com` / `Admin@123`
- Seeded via `OnModelCreating` `HasData` (hardcoded BCrypt hash — do not regenerate)

### Integration test DB

`CustomWebApplicationFactory` replaces the SQLite connection with `DataSource=:memory:` (same SQLite provider, no two-provider conflict). `EnsureCreated()` in `CreateHost` applies `HasData` seed. Each factory instance uses its own in-memory SQLite connection.

---

## SOLID Principles (enforced throughout)

All code in this project **must** follow SOLID. These are non-negotiable:

### Single Responsibility (SRP)
Each class has exactly one reason to change:
- `AuthService` — auth flow only (register/login)
- `UserService` — user CRUD only
- `PasswordHasher` — hashing only
- `TokenService` — JWT generation only
- `UserRepository` — data access only

### Open/Closed (OCP)
New features → new classes. Do not modify working service/endpoint code to add unrelated features. Domain entities expose behavior methods (`UpdateEmail`, `UpdateRole`) instead of public setters.

### Liskov Substitution (LSP)
All interface implementations must be fully substitutable. The integration test factory swaps `UserRepository` for an in-memory-backed version — callers must be unaffected. Never implement only part of an interface.

### Interface Segregation (ISP)
Keep interfaces narrow and single-purpose:
- `IPasswordHasher` — only hash/verify
- `ITokenService` — only generate token
- `IAuthService` — only auth operations
- `IUserService` — only CRUD operations
- `IUserRepository` — only data access

Do NOT create fat interfaces.

### Dependency Inversion (DIP)
All layers depend on **abstractions**, never on concrete classes:
- Application depends on `IUserRepository`, `IPasswordHasher`, `ITokenService` (all interfaces)
- Infrastructure implements those interfaces
- Concrete classes are only wired in `DependencyInjection.cs` and `Program.cs`
- Domain has zero dependencies on other layers

**Rule:** If you're writing `new ConcreteService()` outside of DI registration, something is wrong.
