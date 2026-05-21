# Clean Architecture + SOLID Refactor

**Data:** 2026-05-21  
**Contexto:** Projeto de aprendizado C# + .NET 10. Objetivo: aprender os padrões usados em projetos reais simulando uma estrutura de produção.

---

## Estrutura de Projetos

Solução com 4 projetos `.csproj` dentro de `src/`:

```
aprendendo-api.sln
└── src/
    ├── aprendendo-api.Domain/
    ├── aprendendo-api.Application/
    ├── aprendendo-api.Infrastructure/
    └── aprendendo-api.API/
```

Grafo de dependências (setas = "depende de"):
```
API → Application
API → Infrastructure        (só para registrar implementações no DI)
Application → Domain
Infrastructure → Domain
Infrastructure → Application
Domain → (nada)
```

O compilador força as fronteiras — `Domain` não pode importar EF Core, `Application` não conhece SQLite.

---

## Conteúdo de Cada Camada

### Domain
Zero dependências externas. Define o "o que" do sistema.

```
Entities/
  Todo.cs               ← entidade principal
Interfaces/
  ITodoRepository.cs    ← contrato de acesso a dados (só interface)
```

`Todo.cs` — entidade com propriedades mutáveis (necessário para EF Core):
```csharp
public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateOnly? DueBy { get; set; }
    public bool IsComplete { get; set; }
}
```

`ITodoRepository.cs`:
```csharp
public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> AddAsync(Todo todo);
    Task DeleteAsync(Todo todo);
}
```

### Application
Depende só de Domain. Define os casos de uso.

```
DTOs/
  CreateTodoRequest.cs  ← body do POST (desacopla a API da entidade)
  TodoResponse.cs       ← resposta da API (evita expor entidade diretamente)
Services/
  ITodoService.cs       ← interface do serviço
  TodoService.cs        ← implementação dos use cases
```

`TodoService` usa `ITodoRepository` (injetado via DI), nunca conhece EF Core.

### Infrastructure
Detalhes técnicos. Implementa os contratos definidos em Domain/Application.

```
Persistence/
  AppDbContext.cs       ← EF Core DbContext
  TodoRepository.cs     ← implementa ITodoRepository
  Migrations/           ← geradas por dotnet-ef
```

Referencia pacote `Microsoft.EntityFrameworkCore.Sqlite`.

### API
Ponto de entrada. Orquestra DI e expõe os endpoints.

```
Endpoints/
  TodoEndpoints.cs      ← rotas Minimal API (recebe ITodoService via DI)
Program.cs              ← builder, registro de serviços, app.Run()
```

---

## Fluxo de uma Requisição

Exemplo: `GET /todos/{id}`

```
HTTP Request
    ↓
TodoEndpoints         recebe (int id, ITodoService service)
    ↓
TodoService           chama repository.GetByIdAsync(id)
    ↓
TodoRepository        db.Todos.FindAsync(id)  [EF Core]
    ↓
TodoService           mapeia Todo → TodoResponse
    ↓
TodoEndpoints         TypedResults.Ok(response) | TypedResults.NotFound()
```

---

## SOLID Mapeado

| Princípio | Onde aparece |
|---|---|
| **S** Single Responsibility | `TodoRepository` só acessa dados; `TodoService` só aplica regras de negócio |
| **O** Open/Closed | Novo recurso = nova classe Service/Repository, sem modificar existentes |
| **L** Liskov Substitution | `TodoRepository` satisfaz `ITodoRepository` completamente |
| **I** Interface Segregation | `ITodoRepository` tem só os métodos que `TodoService` precisa |
| **D** Dependency Inversion | `TodoService` depende de `ITodoRepository` (abstração), não de `TodoRepository` (concreção) |

---

## De Para com JS/TS

| C# | JS/TS |
|---|---|
| `TodoEndpoints` | route handler / controller |
| `ITodoService` | interface/type do serviço |
| `TodoService` | `todosService.ts` |
| `ITodoRepository` | interface do repositório |
| `TodoRepository` | `prisma-todos.repo.ts` |
| `AppDbContext` | `PrismaClient` / `DataSource` |
| `CreateTodoRequest` | DTO de entrada (Zod schema input) |
| `TodoResponse` | DTO de saída (tipo de retorno da API) |

---

## Verificação

Após implementar:
1. `dotnet build` na raiz — confirma que as dependências entre projetos estão corretas
2. `dotnet run --project src/aprendendo-api.API` — API sobe normalmente
3. `GET /todos` retorna lista (vazia ou com dados)
4. `POST /todos` + `GET /todos/{id}` funcionam
5. `DELETE /todos/{id}` retorna 204
