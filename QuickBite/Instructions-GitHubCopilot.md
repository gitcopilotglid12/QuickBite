# GitHub Copilot Instructions for QuickBite Food Menu Management Repository

## Project Overview
This repository contains the source code for QuickBite Food Menu Management, a backend API system for restaurant food menu operations. The project is built using .NET 9.0 with ASP.NET Core Web API, Entity Framework Core, and SQL Server. The system provides comprehensive CRUD operations for food items following Test-Driven Development (TDD) methodology.

---

## Key Features
- Food item CRUD operations (Create, Read, Update, Delete)
- Category-based food organization (Appetizers, Main Courses, Desserts, Salads, Soups)
- Dietary tag management (Vegetarian, Vegan, Gluten-Free, etc.)
- RESTful API design with Swagger documentation
- Parameterized queries for SQL injection prevention
- Docker containerization support

---

## Technology Stack
- **Backend:** .NET 9.0 (ASP.NET Core Web API, layered architecture)
- **Database:** SQL Server 2022 (primary), SQLite (development/testing)
- **ORM:** Entity Framework Core 9.0
- **Testing:** XUnit framework with TDD approach
- **API Documentation:** Swagger/OpenAPI 3.0
- **Validation:** FluentValidation
- **Logging:** Serilog structured logging
- **Containerization:** Docker with Docker Compose

---

## API Architecture & Guidelines
- Layered architecture: Controllers, Services, Models, DTOs, Data, Validators
- RESTful API design with proper HTTP status codes
- Patterns: Service Layer, Repository (optional), Dependency Injection
- Entity Framework Core (Code-First), LINQ for data access
- Async/await pattern for all database operations
- Follow SOLID principles and clean code practices
- TDD approach: Write failing tests → Implement → Refactor
- API versioning with `/api/v1/` prefix
- Comprehensive input validation using FluentValidation
- Structured logging with Serilog
- Document API endpoints with Swagger/OpenAPI
- No raw SQL concatenation - use parameterized queries/ORM only

### Project Structure:
```
QuickBite.API/
├── Controllers/
│   ├── FoodItemsController.cs
│   └── HealthController.cs
├── Models/
│   ├── Entities/
│   │   └── FoodItem.cs
│   ├── DTOs/
│   │   ├── FoodItemDto.cs
│   │   ├── CreateFoodItemDto.cs
│   │   └── UpdateFoodItemDto.cs
│   └── Enums/
│       ├── FoodCategory.cs
│       └── DietaryTag.cs
├── Services/
│   ├── Interfaces/
│   │   └── IFoodItemService.cs
│   └── Implementations/
│       └── FoodItemService.cs
├── Data/
│   ├── QuickBiteDbContext.cs
│   ├── Configurations/
│   │   └── FoodItemConfiguration.cs
│   └── Migrations/
├── Validators/
│   ├── CreateFoodItemValidator.cs
│   └── UpdateFoodItemValidator.cs
├── Middleware/
│   ├── ExceptionMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── WebApplicationExtensions.cs
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── Dockerfile

QuickBite.Tests/
├── Unit/
│   ├── Controllers/
│   ├── Services/
│   └── Validators/
├── Integration/
│   └── FoodItemsIntegrationTests.cs
└── TestData/
    └── FoodItemTestData.cs
```

## Food Item Data Model
**Core Entity Structure:**
- `Id` (int): Primary key, auto-generated
- `Name` (string): Required, max 100 characters
- `Description` (string): Optional, max 500 characters  
- `Price` (decimal): Required, positive value with 2 decimal places
- `Category` (FoodCategory enum): Required - Appetizers(1), MainCourses(2), Desserts(3), Salads(4), Soups(5)
- `DietaryTag` (DietaryTag enum): Optional - Vegetarian(1), Vegan(2), GlutenFree(3), DairyFree(4), Keto(5), LowCarb(6), Spicy(7), ContainsNuts(8)
- `CreatedAt` (DateTime): Auto-generated timestamp
- `UpdatedAt` (DateTime): Auto-updated timestamp

**API Endpoints:**
- `GET /api/v1/food-items` - Get all food items (with filtering and pagination)
- `GET /api/v1/food-items/{id}` - Get specific food item
- `POST /api/v1/food-items` - Create new food item
- `PUT /api/v1/food-items/{id}` - Update existing food item
- `DELETE /api/v1/food-items/{id}` - Delete food item

---
## General Rules & Best Practices

- Always ask for clarification or user input before proceeding to the next major step or implementing new features.
- Do not make assumptions or act beyond the requested scope.
- Break tasks into smaller, manageable steps and confirm requirements with the team or requester.
- Avoid being "over smart"—focus on clear, collaborative, and incremental progress.
---

## Must Instructions & Security Notes

- **Never use hardcoded values for secrets, credentials, API keys, or sensitive configuration.**
  Use environment variables and configuration files that are excluded from version control (e.g., `.env`, `appsettings.json`).
- **Do not expose sensitive information in code, logs, or error messages.**
- **Validate all user inputs** to prevent injection attacks (SQL, XSS, etc.).
- **Follow secure coding practices** and regularly review for vulnerabilities.
- **CRITICAL: Use only parameterized queries or Entity Framework ORM - NO raw SQL concatenation allowed.**
- **Keep dependencies up to date** and monitor for known security issues.
- **Do not commit any files containing secrets or credentials.**
- **Ensure all code complies with open-source licenses and legal requirements.**
- **Follow TDD methodology strictly: Red (failing test) → Green (minimal implementation) → Refactor.**

---

## Contribution Guidelines
- Fork the repository, create feature branches
- Ensure all tests pass before merging
- Adhere to coding standards and guidelines
- Use CI/CD pipeline for deployments

---

## Useful Commands
- `dotnet new webapi -n QuickBite.API` (create API project)
- `dotnet new xunit -n QuickBite.Tests` (create test project)
- `dotnet restore` / `dotnet build` / `dotnet run` (backend)
- `dotnet ef migrations add <MigrationName>` (create migration)
- `dotnet ef database update` (apply migrations)
- `dotnet test` / `dotnet test --collect:"XPlat Code Coverage"` (testing)
- `dotnet watch run` (development with hot reload)
- `docker build -t quickbite-api .` (build Docker image)
- `docker-compose up -d` (start with SQL Server)

---

## Resources
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [XUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/)
- [Swagger/OpenAPI Documentation](https://swagger.io/docs/)
- [Serilog Documentation](https://serilog.net/)

---

**For questions or support, contact the Engineering Lead or open an issue in the repository.**
---

## Backend Folder Structure Example

```
QuickBite.sln
QuickBite.API/
  Controllers/
    FoodItemsController.cs
    HealthController.cs
  Models/
    Entities/
      FoodItem.cs
    DTOs/
      FoodItemDto.cs
      CreateFoodItemDto.cs
      UpdateFoodItemDto.cs
    Enums/
      FoodCategory.cs
      DietaryTag.cs
  Services/
    Interfaces/
      IFoodItemService.cs
    Implementations/
      FoodItemService.cs
  Data/
    QuickBiteDbContext.cs
    Configurations/
      FoodItemConfiguration.cs
    Migrations/
  Validators/
    CreateFoodItemValidator.cs
    UpdateFoodItemValidator.cs
  Middleware/
    ExceptionMiddleware.cs
  Extensions/
    ServiceCollectionExtensions.cs
  Program.cs
  appsettings.json
  QuickBite.API.csproj
  Dockerfile
QuickBite.Tests/
  Unit/
    Controllers/
      FoodItemsControllerTests.cs
    Services/
      FoodItemServiceTests.cs
    Validators/
      FoodItemValidatorTests.cs
  Integration/
    FoodItemsIntegrationTests.cs
  TestData/
    FoodItemTestData.cs
  QuickBite.Tests.csproj
docker-compose.yml
.gitignore
README.md
```