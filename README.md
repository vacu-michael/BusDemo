# BusDemo

BusDemo is a demo .NET application demonstrating how to trigger a MassTransit workflow connected to Azure Service Bus from a frontend application. The solution follows Clean Architecture principles, with separate projects for each layer:

- **Frontend**: Blazor WebAssembly UI for user interaction
- **BLL**: Business Logic Layer
- **DAL**: Entity Framework Core context for Azure SQL Database
- **SAL**: Service Access Layer (integration, external services)
- **Models**: Shared domain models and contracts
- **Worker**: Background service for workflow processing

## Features

- Clean Architecture with clear separation of concerns
- MassTransit integration with Azure Service Bus
- EF Core for data access to Azure SQL DB
- Modern C# syntax and code style

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Azure Service Bus instance
- Azure SQL Database

## Getting Started

1. **Clone the repository**
2. **Configure connection strings** in `appsettings.json` for both Azure SQL and Service Bus in `Frontend`, `Worker`, and `DAL` projects.
3. **Restore dependencies**:

   ```pwsh
   dotnet restore
   ```

4. **Apply EF Core migrations** (if needed):

   ```pwsh
   dotnet ef database update --project DAL
   ```

5. **Run the solution**:

   ```pwsh
   dotnet build
   dotnet run --project Frontend
   dotnet run --project Worker
   ```

## Code Style Guidelines

- Use latest C# syntax and features
- Always use namespace declarations (not namespace scopes)
- Prefer early returns over nested if statements

## Project Structure

```
BusDemo.sln
BLL/        # Business Logic Layer
DAL/        # Data Access Layer (EF Core)
SAL/        # Service Access Layer
Models/     # Shared Models
Frontend/   # Blazor WebAssembly Frontend
Worker/     # Background Worker
```

## License

This project is for demo purposes only.
