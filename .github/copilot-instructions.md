# GitHub Copilot Instructions

## General Coding Guidelines

- **Use the latest C# syntax and features** available for .NET 9 and C# 13 (or latest supported).
- **Always use namespace declarations** (e.g., `namespace MyApp;`) instead of namespace scopes (curly braces).
- **Prefer early returns** in methods/functions to reduce nesting and improve readability.
- **Follow Clean Architecture principles**: keep business logic, data access, service access, and models in their respective projects.
- **Use modern .NET idioms**: pattern matching, target-typed new, records, etc.
- **Consistent code style**: use expression-bodied members where appropriate, prefer `var` for local variables unless explicit type improves clarity.
- **Prefer adding NuGet packages and project references via the `dotnet` CLI** (e.g., `dotnet add package`, `dotnet add reference`) instead of manual file edits.
- **Always run `dotnet` CLI commands without explicit version numbers** to ensure the latest compatible version is installed, unless a specific version is required.

## Project Structure

- `Frontend`: Blazor WebAssembly UI
- `BLL`: Business Logic Layer
- `DAL`: Entity Framework Core context for Azure SQL
- `SAL`: Service Access Layer
- `Models`: Shared domain models
- `Worker`: Background service

## Additional Notes

- When generating code, always check for nulls and handle exceptions appropriately.
- Use dependency injection for all services.
- Write clear, concise XML documentation for public APIs.
