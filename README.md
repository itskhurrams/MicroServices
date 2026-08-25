# Micro Services Implementation with .NET

Micro Services implementation with .NET 10, Entity Framework Core, MediatR and a RabbitMQ event bus.

## Solution layout

- **MicroServices.Banking.API** / **MicroServices.Transfer.API** — ASP.NET Core Web APIs (one per bounded context)
- **MicroServices.Banking.Application** / **MicroServices.Transfer.Application** — application services
- **MicroServices.Banking.Domain** / **MicroServices.Transfer.Domain** — commands, events, event handlers, domain models
- **MicroServices.Banking.Data** / **MicroServices.Transfer.Data** — EF Core `DbContext`s, repositories, migrations
- **MicroServices.Domain.Core** — shared command/event/bus abstractions
- **MicroServices.Infrastructure.Bus** — RabbitMQ-backed `IEventBus` implementation
- **MicroServices.Infrastructure.IOC** — composition root wiring up dependency injection for both APIs

The two APIs communicate through domain events published to RabbitMQ: `Banking.API` publishes `TransferCreatedEvent`, which `Transfer.API` subscribes to and persists as a `TransferLog`.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (each API has its own database)
- RabbitMQ (default `localhost:5672`, guest/guest)

## Configuration

Connection strings are **not** committed to source control. Each API reads `ConnectionStrings:BankingDbConnection` / `ConnectionStrings:TransferDbConnection` from configuration, which resolves via (in order) environment variables, [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), and `appsettings.json`.

Set your local connection string with User Secrets, e.g. for `MicroServices.Banking.API`:

```
dotnet user-secrets set "ConnectionStrings:BankingDbConnection" "Server=YOUR_SERVER;Database=MicroServices_BankingDB;User ID=...;Pwd=...;MultipleActiveResultSets=true" --project MicroServices/MicroServices.Banking.API
```

and the equivalent `ConnectionStrings:TransferDbConnection` for `MicroServices.Transfer.API`.

RabbitMQ connection details are configurable via the `RabbitMQ` section in `appsettings.json` (defaults to `localhost`):

```json
"RabbitMQ": {
  "HostName": "localhost",
  "Port": 5672,
  "UserName": "guest",
  "Password": "guest",
  "VirtualHost": "/"
}
```

## Running

```
dotnet build MicroServices/MicroServices.sln
dotnet run --project MicroServices/MicroServices.Banking.API
dotnet run --project MicroServices/MicroServices.Transfer.API
```

Each API exposes Swagger UI at `/swagger` when running in Development.

## Applying migrations

```
dotnet ef database update --project MicroServices/MicroServices.Banking.Data --startup-project MicroServices/MicroServices.Banking.API
dotnet ef database update --project MicroServices/MicroServices.Transfer.Data --startup-project MicroServices/MicroServices.Transfer.API
```

## Key dependencies

| Package | Version |
|---|---|
| .NET | 10.0 |
| Entity Framework Core | 10.0.11 |
| MediatR | 12.5.0 |
| RabbitMQ.Client | 7.2.2 |
| Swashbuckle.AspNetCore | 10.2.3 |
