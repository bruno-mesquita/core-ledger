<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:0f0c29,50:302b63,100:24243e&height=200&section=header&text=CoreLedger&fontSize=72&fontColor=ffffff&fontAlignY=38&desc=A%20Banking%20Platform%20to%20Learn%20C%23&descAlignY=58&descSize=20&animation=fadeIn" width="100%" />
</p>

<p align="center">
  <img src="https://readme-typing-svg.demolab.com?font=Fira+Code&size=18&duration=3000&pause=800&color=A78BFA&center=true&vCenter=true&multiline=false&width=600&lines=Learning+C%23+%C2%B7+.NET+10+%C2%B7+Clean+Architecture;Domain-Driven+Design+%C2%B7+CQRS+%C2%B7+MediatR;JWT+Auth+%C2%B7+EF+Core+%C2%B7+PostgreSQL;Testcontainers+%C2%B7+Integration+Tests" alt="Typing SVG" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/Next.js-16-000000?style=for-the-badge&logo=next.js&logoColor=white" />
  <img src="https://img.shields.io/badge/Docker-ready-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/License-MIT-22c55e?style=for-the-badge" />
</p>

<br/>

## 📖 About

**CoreLedger** is a modular banking platform built as a **learning project for C# and .NET**. It covers real-world backend engineering — from domain modeling and CQRS to JWT authentication and containerized integration tests.

The goal isn't to ship a product. The goal is to deeply understand how production-grade .NET systems are built.

**What this project practices:**

| Concept | Implementation |
|---|---|
| Clean Architecture | Strict inward dependency rule: `API → Infrastructure → Application → Domain → SharedKernel` |
| Domain-Driven Design | Aggregates (`User`, `Account`, `Transaction`, `LedgerEntry`) with rich domain behavior |
| CQRS | Every use case is a `Command` or `Query` handled by MediatR |
| Result Pattern | Handlers return `Result<T>` — no business exceptions thrown |
| Validation Pipeline | FluentValidation as MediatR behavior → automatic 400 responses |
| JWT + Refresh Tokens | Stateless auth with secure token rotation |
| EF Core + PostgreSQL | Code-first schema, Npgsql provider, migrations |
| Integration Testing | Real PostgreSQL via Testcontainers — no mocks for the database |

<br/>

## 🏗️ Architecture

```mermaid
graph TD
    A["🌐 CoreLedger.API<br/><sub>Minimal APIs · Serilog · OpenAPI</sub>"]
    B["🏭 CoreLedger.Infrastructure<br/><sub>EF Core · Repositories · JWT · BCrypt</sub>"]
    C["⚙️ CoreLedger.Application<br/><sub>Commands · Queries · Handlers · DTOs</sub>"]
    D["🏛️ CoreLedger.Domain<br/><sub>Aggregates · Repository Interfaces · Events</sub>"]
    E["🔩 CoreLedger.SharedKernel<br/><sub>Entity · AggregateRoot · Result&lt;T&gt; · IDomainEvent</sub>"]

    A -->|"depends on"| B
    B -->|"depends on"| C
    C -->|"depends on"| D
    D -->|"depends on"| E

    style A fill:#7c3aed,color:#fff,stroke:#6d28d9
    style B fill:#2563eb,color:#fff,stroke:#1d4ed8
    style C fill:#0891b2,color:#fff,stroke:#0e7490
    style D fill:#059669,color:#fff,stroke:#047857
    style E fill:#374151,color:#fff,stroke:#1f2937
```

<br/>

## ✨ Features

- 🔐 **JWT Authentication** — register, login, token refresh with rotation
- 🏦 **Account Management** — create, query, close accounts with balance tracking
- 💸 **Transfers** — idempotent transfers between accounts with duplicate protection
- 📒 **Double-Entry Ledger** — every transaction creates balanced `LedgerEntry` records
- 📄 **Pagination** — transactions endpoint with `page` + `pageSize` params
- 🛡️ **Validation Pipeline** — FluentValidation wired as MediatR behavior
- 🐳 **Docker Ready** — full stack via `docker compose up`
- 🧪 **Integration Tests** — Testcontainers spins up real PostgreSQL per test class

<br/>

## 🛠️ Tech Stack

### Backend

| Technology | Version | Purpose |
|---|---|---|
| .NET / ASP.NET Core | 10 | Runtime + Minimal APIs |
| MediatR | 14 | CQRS bus + pipeline behaviors |
| Entity Framework Core | 10 | ORM + migrations |
| Npgsql | latest | PostgreSQL EF provider |
| FluentValidation | 12 | Request validation |
| BCrypt.Net | 4 | Password hashing |
| JWT Bearer | 8 | Stateless authentication |
| Serilog | 10 | Structured logging |

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| Next.js | 16 | React framework (App Router) |
| React | 19 | UI library |
| TanStack Query | 5 | Server state management |
| React Hook Form + Zod | 7 / 4 | Forms + schema validation |
| Radix UI + Tailwind CSS | — | Accessible components + styling |

### Testing & DevOps

| Technology | Purpose |
|---|---|
| NSubstitute | Unit test mocks (domain + handlers) |
| Testcontainers | Real PostgreSQL for integration tests |
| Docker + Compose | Container orchestration |
| Make | Task runner (`make dev`, `make test`) |

<br/>

## 🚀 Getting Started

### Option A — Docker (full stack)

```bash
# Clone
git clone https://github.com/bmesquita196/aprendendo-api.git
cd aprendendo-api

# Start everything
docker compose up --build
```

API available at `http://localhost:5000` · Frontend at `http://localhost:3000`

---

### Option B — Local development

**Prerequisites:** .NET 10 SDK, Docker (for PostgreSQL), Node.js 20+, pnpm

```bash
# 1. Start PostgreSQL
docker compose up postgres -d

# 2. Apply database migrations
dotnet ef database update \
  --project src/CoreLedger.Infrastructure \
  --startup-project src/CoreLedger.API

# 3. Run the API
dotnet run --project src/CoreLedger.API/CoreLedger.API.csproj

# 4. Run the frontend (separate terminal)
cd src/frontend
pnpm install && pnpm dev
```

Swagger UI: `http://localhost:5000/swagger`

**Default admin account (dev only):**
```
Email:    admin@coreledger.com
Password: Admin@123
```

<br/>

## 🔌 API Reference

<details>
<summary><strong>🔐 Auth</strong></summary>

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `POST` | `/auth/register` | Register new user | ❌ |
| `POST` | `/auth/login` | Login → returns JWT + refresh token | ❌ |
| `POST` | `/auth/refresh` | Rotate refresh token → new JWT | ❌ |

**Login response:**
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "abc123...",
  "expiresIn": 3600
}
```

</details>

<details>
<summary><strong>🏦 Accounts</strong></summary>

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `POST` | `/accounts` | Create account | ✅ |
| `GET` | `/accounts/{id}` | Get account details | ✅ |
| `GET` | `/accounts/{id}/balance` | Get current balance | ✅ |
| `GET` | `/accounts/{id}/transactions` | List transactions (paginated) | ✅ |
| `DELETE` | `/accounts/{id}` | Close account | ✅ |

**Pagination params:** `?page=1&pageSize=20`

</details>

<details>
<summary><strong>💸 Transfers</strong></summary>

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| `POST` | `/transfers` | Create transfer | ✅ |

**Request body:**
```json
{
  "sourceAccountId": "uuid",
  "destinationAccountId": "uuid",
  "amount": 100.00,
  "description": "Payment",
  "idempotencyKey": "unique-key-per-request"
}
```

> `idempotencyKey` prevents duplicate transfers — safe to retry on network failures.

</details>

<br/>

## 📁 Project Structure

```
aprendendo-api/
├── src/
│   ├── CoreLedger.SharedKernel/       # Entity, AggregateRoot, Result<T>, IDomainEvent
│   ├── CoreLedger.Domain/             # Aggregates + repository interfaces + domain events
│   ├── CoreLedger.Application/        # Commands, Queries, Handlers, DTOs, service interfaces
│   ├── CoreLedger.Infrastructure/     # EF Core, repositories, JWT, BCrypt implementations
│   ├── CoreLedger.API/                # Minimal API endpoints, Program.cs, middleware
│   └── frontend/                      # Next.js 16 app (App Router)
│
├── tests/
│   ├── CoreLedger.UnitTests/          # Domain logic + handler tests (NSubstitute, no DB)
│   └── CoreLedger.IntegrationTests/   # Endpoint tests (Testcontainers PostgreSQL)
│
├── docker-compose.yml
├── Dockerfile
├── Makefile
└── CoreLedger.slnx
```

<br/>

## 🧪 Testing

```bash
# Unit tests only (fast, no Docker needed)
dotnet test tests/CoreLedger.UnitTests/

# Integration tests (Docker required — Testcontainers auto-manages containers)
dotnet test tests/CoreLedger.IntegrationTests/

# All tests
dotnet test

# Single test by name
dotnet test --filter "FullyQualifiedName~CreateTransfer_ShouldDebitSource"
```

> Integration tests use **Testcontainers** to spin up a real PostgreSQL container per test class and tear it down automatically. No shared state, no mocks.

<br/>

## 📐 Design Principles

- **SRP** — each Handler has one reason to change; repositories do data access only
- **OCP** — new use case → new Command + Handler; existing handlers never modified
- **LSP** — all repository implementations fully substitutable (NSubstitute validates this)
- **ISP** — `IPasswordHasher`, `ITokenService`, `IUnitOfWork` are single-purpose interfaces
- **DIP** — Application depends on interfaces only; Infrastructure wires the implementations

> **Rule enforced:** `new ConcreteService()` outside DI registration is forbidden.

<br/>

<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=0:24243e,50:302b63,100:0f0c29&height=120&section=footer&animation=fadeIn" width="100%" />
</p>

<p align="center">
  <sub>Built to learn C# · by <strong>Bruno Mesquita</strong></sub>
</p>
