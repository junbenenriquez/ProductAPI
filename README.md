# ProductAPI

A sample ASP.NET Core Web API demonstrating Clean Architecture, JWT authentication, CRUD operations, and unit tests.

## Features

- Clean Architecture (API, Application, Domain, Infrastructure)
- JWT authentication
- Products CRUD API
- Soft delete with audit fields
- Unit tests with xUnit, Moq, FluentAssertions

# Setup

1. Copy sample config:

   ```bash
   cp appsettings.Sample.json appsettings.json
2. Update connection string and JWT secret in appsettings.json.

3. Run migrations and start API:

   dotnet ef database 
   
   dotnet run


4. Run tests:

   dotnet test
