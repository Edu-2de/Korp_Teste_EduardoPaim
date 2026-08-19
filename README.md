# Korp Teste — Eduardo Paim

Sistema de controle de estoque e emissão de notas fiscais, implementado como
um conjunto de microsserviços em .NET 9 com um front-end em Angular.

- **Inventory.API** — cadastro de produtos e controle de saldo em estoque.
- **Billing.API** — criação, composição e emissão (impressão) de notas fiscais.
- **Gateway** — reverse proxy (YARP) que expõe as duas APIs sob uma única origem.
- **Frontend** — SPA em Angular que consome o Gateway.

Cada API de domínio tem seu próprio banco PostgreSQL — não há banco
compartilhado entre Inventory e Billing.

## Arquitetura

```text
                            ┌──────────────┐
                 4200/80    │   Frontend   │
        Browser ───────────▶│ (Angular +   │
                            │  Nginx)      │
                            └──────┬───────┘
                                   │ /api/*
                                   ▼
                            ┌──────────────┐
                     8080   │   Gateway    │
                            │   (YARP)     │
                            └──┬────────┬──┘
                   /api/products   /api/invoices
                       ▼                ▼
              ┌────────────────┐ ┌────────────────┐
              │ Inventory.API  │◀│  Billing.API   │
              │   (porta 5254) │ │  (porta 5298)  │
              └───────┬────────┘ └───────┬────────┘
                      ▼                  ▼
              ┌───────────────┐  ┌───────────────┐
              │ inventory_db  │  │  billing_db   │
              │  (Postgres)   │  │  (Postgres)   │
              └───────────────┘  └───────────────┘
```

Na emissão de uma nota fiscal (`POST /api/invoices/{id}/print`), o
Billing.API chama o Inventory.API (`PATCH /api/products/{id}/decrease-balance`)
para debitar o saldo de cada produto — comunicação síncrona via HTTP, com
Polly (retry exponencial + circuit breaker) e uma chave de idempotência por
item, evitando débito duplicado em caso de reenvio.

## Stack técnica

#### Backend

- .NET 9 / C# 12, ASP.NET Core Web API
- Entity Framework Core 9 + Npgsql (PostgreSQL)
- YARP (Yarp.ReverseProxy) para o Gateway
- Polly (retry + circuit breaker) na comunicação Billing → Inventory
- Serilog (logging estruturado) + `CorrelationId` (rastreamento de requisições entre serviços)
- Swashbuckle / Swagger para documentação da API
- xUnit + Testcontainers (Postgres real em container) para testes de integração dos controllers; testes unitários simples para o domínio

#### Frontend

- Angular 21 (standalone components, signals)
- PrimeNG 21 + Tailwind CSS v4
- TanStack Query (`@tanstack/angular-query-experimental`) para cache/estado de servidor
- Vitest para testes

#### Infra

- Docker + Docker Compose (todos os serviços, incluindo os dois bancos e o frontend)
- Nginx servindo o build estático do Angular e fazendo proxy de `/api/*` para o Gateway

## Como rodar (Docker — recomendado)

Pré-requisitos: Docker Desktop.

```bash
cd backend
docker compose up --build
```

Isso sobe 6 containers: `frontend`, `gateway`, `inventory-api`, `billing-api`,
`inventory-db` e `billing-db`, todos na mesma rede (`korp-network`).

| Serviço           | URL                             |
|-------------------|---------------------------------|
| Frontend          | <http://localhost:4200>         |
| Gateway (API)     | <http://localhost:8080/api/...> |
| Inventory Swagger | <http://localhost:5254/swagger> |
| Billing Swagger   | <http://localhost:5298/swagger> |

O frontend fala apenas com sua própria origem (`/api/...`); é o Nginx do
container `frontend` quem faz o proxy reverso para o Gateway — não há CORS
nem URL de API hardcoded no bundle de produção.

As migrations do EF Core rodam automaticamente na subida de cada API
(`Database.MigrateAsync()`), então não é necessário nenhum passo manual de
banco de dados.

Para derrubar tudo:

```bash
docker compose down
```

Para remover também os volumes dos bancos (dados persistidos):

```bash
docker compose down -v
```

## Como rodar localmente (sem Docker)

Pré-requisitos: .NET 9 SDK, Node.js 22+, PostgreSQL local (ou dois
containers Postgres avulsos) escutando em `localhost:5432`, com os bancos
`inventory_db` e `billing_db` criados e usuário `postgres`/senha `postgres`
(ou ajuste `appsettings.Development.json` de cada API).

```bash
# Inventory.API — http://localhost:5254
cd backend/src/Inventory.API
dotnet run

# Billing.API — http://localhost:5298 (em outro terminal)
cd backend/src/Billing.API
dotnet run

# Gateway — http://localhost:8080 (em outro terminal)
cd backend/src/Gateway
dotnet run

# Frontend — http://localhost:4200 (em outro terminal)
cd frontend
npm install
npm start
```

Em ambiente `Development`, cada API já expõe o Swagger em `/swagger` e roda
suas próprias migrations automaticamente ao iniciar.

## Testes

#### Backend

Na raiz de `backend/`:

```bash
dotnet test
```

Os testes de integração dos controllers usam Testcontainers, ou seja, sobem
um Postgres real em Docker durante a execução — é necessário o Docker
rodando para `dotnet test` funcionar. Se as duas suítes (`Inventory.API.Tests`
e `Billing.API.Tests`) forem executadas em paralelo com poucos recursos
disponíveis, pode ocorrer contenção de memória; nesse caso, rode-as
separadamente:

```bash
dotnet test tests/Inventory.API.Tests
dotnet test tests/Billing.API.Tests
```

#### Frontend

Na raiz de `frontend/`:

```bash
ng test --watch=false
```

## Endpoints principais

### Inventory.API — `/api/products`

| Método | Rota                                  | Descrição                                          |
|--------|---------------------------------------|----------------------------------------------------|
| GET    | `/api/products`                       | Lista todos os produtos                            |
| GET    | `/api/products/{id}`                  | Busca um produto por id                            |
| POST   | `/api/products`                       | Cria um produto (`code`, `description`, `balance`) |
| PATCH  | `/api/products/{id}/description`      | Atualiza a descrição                               |
| PATCH  | `/api/products/{id}/balance`          | Atualiza o saldo (valor absoluto)                  |
| PATCH  | `/api/products/{id}/decrease-balance` | Debita saldo (requer header `X-Idempotency-Key`)   |
| DELETE | `/api/products/{id}`                  | Desativa o produto (soft delete)                   |

### Billing.API — `/api/invoices`

| Método | Rota                                | Descrição                                              |
|--------|-------------------------------------|--------------------------------------------------------|
| GET    | `/api/invoices`                     | Lista todas as notas fiscais, com seus itens           |
| GET    | `/api/invoices/{id}`                | Busca uma nota fiscal por id                           |
| POST   | `/api/invoices`                     | Cria uma nota vazia, com status `Open`                 |
| POST   | `/api/invoices/{id}/items`          | Adiciona um item (`productId`, `quantity`)             |
| DELETE | `/api/invoices/{id}/items/{itemId}` | Remove um item (só com a nota `Open`)                  |
| DELETE | `/api/invoices/{id}`                | Exclui a nota inteira (só com a nota `Open`)           |
| POST   | `/api/invoices/{id}/print`          | Emite a nota: debita estoque e fecha a nota (`Closed`) |

Todas as rotas acima também ficam disponíveis através do Gateway, prefixadas
pela mesma origem (`http://localhost:8080/api/...`).

## Decisões de design

- **Soft delete de produto**: `DELETE /api/products/{id}` não apaga a linha,
  apenas marca `IsActive = false`. Produtos inativos somem da listagem
  disponível para novas notas fiscais, mas continuam existindo para resolver
  descrição/código em notas que já os referenciam — uma nota fiscal fechada
  nunca perde a rastreabilidade dos produtos que a compõem.
- **Idempotência no débito de estoque**: cada item de uma nota gera uma
  `X-Idempotency-Key` determinística (`invoice-{invoiceId}-item-{itemId}`).
  Se a chamada Billing → Inventory for reenviada (timeout, retry do Polly),
  o Inventory.API reconhece a chave já processada e não debita duas vezes.
- **Concorrência otimista**: `Product` usa a coluna de sistema `xmin` do
  Postgres como token de concorrência (`IsRowVersion()`). Uma atualização
  concorrente no mesmo produto retorna `409 Conflict` em vez de sobrescrever
  silenciosamente ou estourar um erro genérico.
- **Nota fiscal só é fechada com itens**: `Close()` recusa fechar uma nota
  sem nenhum item — evita nota fiscal vazia sendo emitida.
- **Resiliência HTTP**: o `HttpClient` do Billing.API para o Inventory.API
  tem retry exponencial (3 tentativas) e circuit breaker (abre após 3 falhas
  consecutivas, por 30s) via Polly.
- **Correlation ID**: todo request recebe/propaga um `X-Correlation-ID`
  entre Gateway → Billing.API → Inventory.API, útil para rastrear uma
  operação de ponta a ponta nos logs (Serilog).

## Estrutura do repositório

```text
backend/
  src/
    Inventory.API/     # domínio de produtos e estoque
    Billing.API/        # domínio de notas fiscais
    Gateway/             # reverse proxy YARP
    Shared.Kernel/        # middlewares e extensões cross-cutting
  tests/
    Inventory.API.Tests/
    Billing.API.Tests/
  docker-compose.yml

frontend/
  src/app/
    core/                # services, models, http client, utils
    features/
      products/           # listagem, criação e edição de produtos
      invoices/            # listagem, criação e composição de notas
  Dockerfile
  nginx.conf
```
