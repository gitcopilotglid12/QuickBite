# Technical Documentation

## QuickBite-AI-App Food Menu Management System

### Document Information

- **Project Name**: QuickBite-AI-App Food Menu Management System
- **Document Type**: Technical Documentation
- **Version**: 1.0
- **Date**: September 20, 2025
- **Technology Stack**: .NET 9.0, Entity Framework Core, SQL Server, Docker, Swagger, XUnit

---

## Table of Contents

1. [System Architecture](#1-system-architecture)
2. [Technology Stack](#2-technology-stack)
3. [Database Design](#3-database-design)
4. [API Design](#4-api-design)
5. [Security Implementation](#5-security-implementation)
6. [Testing Strategy](#6-testing-strategy)
7. [Development Setup](#7-development-setup)
8. [Deployment Configuration](#8-deployment-configuration)
9. [Code Standards](#9-code-standards)
10. [Performance Considerations](#10-performance-considerations)

---

## 1. System Architecture

### 1.1 High-Level Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Client Apps   │    │   API Gateway   │    │   Load Balancer │
│  (Frontend/     │◄──►│   (Optional)    │◄──►│   (Optional)    │
│   Mobile)       │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    QuickBite-AI-App API (.NET 9.0)                    │
├─────────────────────────────────────────────────────────────────┤
│  Controllers Layer                                              │
│  ├── FoodItemsController                                        │
│  └── HealthController                                           │
├─────────────────────────────────────────────────────────────────┤
│  Business Logic Layer                                           │
│  ├── Services (IFoodItemService)                               │
│  ├── DTOs (Data Transfer Objects)                              │
│  └── Validators                                                │
├─────────────────────────────────────────────────────────────────┤
│  Data Access Layer                                             │
│  ├── DbContext (QuickBite-AI-AppDbContext)                           │
│  ├── Entities (FoodItem)                                      │
│  └── Repositories (Optional)                                   │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    SQL Server Database                         │
│  ├── food_items table                                         │
│  ├── Indexes                                                  │
│  └── Constraints                                              │
└─────────────────────────────────────────────────────────────────┘
```

### 1.2 Project Structure

```
QuickBite-AI-App.API/
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
│   ├── QuickBite-AI-AppDbContext.cs
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

QuickBite-AI-App.Tests/
├── Unit/
│   ├── Controllers/
│   │   └── FoodItemsControllerTests.cs
│   ├── Services/
│   │   └── FoodItemServiceTests.cs
│   └── Validators/
│       └── FoodItemValidatorTests.cs
├── Integration/
│   ├── FoodItemsIntegrationTests.cs
│   └── TestFixtures/
│       └── WebApplicationFactory.cs
└── TestData/
    └── FoodItemTestData.cs
```

---

## 2. Technology Stack

### 2.1 Core Technologies

| Component         | Technology            | Version | Purpose                  |
| ----------------- | --------------------- | ------- | ------------------------ |
| Runtime           | .NET                  | 9.0     | Application runtime      |
| Web Framework     | ASP.NET Core          | 9.0     | Web API framework        |
| ORM               | Entity Framework Core | 9.0     | Database access          |
| Database          | SQL Server            | 2022    | Data persistence         |
| Testing           | XUnit                 | 2.8+    | Unit/Integration testing |
| API Documentation | Swagger/OpenAPI       | 3.0     | API documentation        |
| Containerization  | Docker                | Latest  | Application deployment   |

### 2.2 Additional Components

- **FluentValidation**: Input validation and business rules
- **AutoMapper**: Object-to-object mapping
- **Serilog**: Structured logging framework
- **Response Caching**: Performance optimization
- **Health Checks**: Application monitoring
- **API Versioning**: Version management

---

## 3. Database Design

### 3.1 Entity Model

**FoodItem Entity Structure:**

- `Id` (int): Primary key, auto-generated
- `Name` (string): Required, max 100 characters
- `Description` (string): Optional, max 500 characters
- `Price` (decimal): Required, positive value with 2 decimal places
- `Category` (FoodCategory enum): Required food category
- `DietaryTag` (DietaryTag enum): Optional dietary information
- `CreatedAt` (DateTime): Auto-generated timestamp
- `UpdatedAt` (DateTime): Auto-updated timestamp

**Food Categories:**

- Appetizers (1)
- MainCourses (2)
- Desserts (3)
- Salads (4)
- Soups (5)

**Dietary Tags:**

- Vegetarian (1), Vegan (2), GlutenFree (3), DairyFree (4)
- Keto (5), LowCarb (6), Spicy (7), ContainsNuts (8)

### 3.2 Database Schema

```sql
CREATE TABLE food_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(500) NULL,
    price DECIMAL(10,2) NOT NULL CHECK (price > 0),
    category INT NOT NULL,
    dietary_tag INT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_food_items_category
        FOREIGN KEY (category) REFERENCES food_categories(id),
    CONSTRAINT FK_food_items_dietary_tag
        FOREIGN KEY (dietary_tag) REFERENCES dietary_tags(id)
);

-- Indexes for performance
CREATE INDEX IX_food_items_category ON food_items(category);
CREATE INDEX IX_food_items_dietary_tag ON food_items(dietary_tag);
CREATE INDEX IX_food_items_name ON food_items(name);
```

### 3.3 Entity Framework Configuration

**Configuration Requirements:**

- Table mapping to `food_items`
- Primary key configuration with auto-increment
- String length constraints (Name: 100 chars, Description: 500 chars)
- Decimal precision for Price (10,2)
- Enum to integer conversion for Category and DietaryTag
- Default values for CreatedAt and UpdatedAt timestamps
- Proper column naming (snake_case) to match database schema

---

## 4. API Design

### 4.1 API Endpoints

| Method | Endpoint                  | Description          | Request Body        | Response            |
| ------ | ------------------------- | -------------------- | ------------------- | ------------------- |
| GET    | `/api/v1/food-items`      | Get all food items   | -                   | `List<FoodItemDto>` |
| GET    | `/api/v1/food-items/{id}` | Get food item by ID  | -                   | `FoodItemDto`       |
| POST   | `/api/v1/food-items`      | Create new food item | `CreateFoodItemDto` | `FoodItemDto`       |
| PUT    | `/api/v1/food-items/{id}` | Update food item     | `UpdateFoodItemDto` | `FoodItemDto`       |
| DELETE | `/api/v1/food-items/{id}` | Delete food item     | -                   | `204 No Content`    |

### 4.2 Data Transfer Objects

**FoodItemDto Structure:**

- All FoodItem properties for read operations
- Category and DietaryTag as string representations
- Includes timestamps

**CreateFoodItemDto Structure:**

- Name, Description, Price, Category, DietaryTag
- Excludes Id and timestamps (auto-generated)

**UpdateFoodItemDto Structure:**

- All properties optional for partial updates
- Excludes Id and timestamps

### 4.3 Controller Implementation

**FoodItemsController Requirements:**

- API versioning: `/api/v1/food-items`
- JSON response format
- Proper HTTP status codes
- Swagger documentation attributes
- Dependency injection for services and logging
- Async/await pattern for all operations
- Input validation with model binding
- Error handling with appropriate responses

**Action Methods:**

- `GetFoodItems`: Supports filtering and pagination
- `GetFoodItem`: Single item retrieval by ID
- `CreateFoodItem`: Creates new food item with validation
- `UpdateFoodItem`: Partial/full update support
- `DeleteFoodItem`: Soft or hard delete operation

---

## 5. Security Implementation

### 5.1 Input Validation

**Validation Requirements:**

- **Name**: Required, max 100 characters, alphanumeric with special chars
- **Description**: Optional, max 500 characters when provided
- **Price**: Required, positive value, range 0-10,000, decimal precision
- **Category**: Required, must be valid enum value
- **DietaryTag**: Optional, must be valid enum when provided

**Validation Tools:**

- FluentValidation for complex validation rules
- Model binding validation attributes
- Custom validation for business rules

### 5.2 SQL Injection Prevention

- **Entity Framework Core**: Uses parameterized queries automatically
- **No Raw SQL**: Avoid `FromSqlRaw` unless absolutely necessary
- **Validation**: All inputs validated before database operations

### 5.3 Error Handling Middleware

**Error Handling Requirements:**

- Global exception handling middleware
- Structured error responses
- Appropriate HTTP status codes mapping
- Security-conscious error messages (no sensitive data exposure)
- Comprehensive logging of exceptions
- Development vs Production error detail levels

**Status Code Mapping:**

- ValidationException → 400 Bad Request
- UnauthorizedAccessException → 401 Unauthorized
- ArgumentException → 400 Bad Request
- KeyNotFoundException → 404 Not Found
- General exceptions → 500 Internal Server Error

---

## 6. Testing Strategy

### 6.1 Test Structure

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test API endpoints with database
- **Contract Tests**: Validate API contracts with Swagger

### 6.2 Unit Test Example

**Unit Testing Approach:**

- Test individual components in isolation
- Mock external dependencies (DbContext, IMapper)
- Test business logic validation
- Cover edge cases and error conditions
- Arrange-Act-Assert pattern
- Test data builders for consistent test data

**Test Categories:**

- Service layer tests
- Controller tests
- Validator tests
- Repository tests (if implemented)

### 6.3 Integration Test Example

**Integration Testing Approach:**

- Test complete API endpoints with database
- Use WebApplicationFactory for in-memory testing
- Test HTTP requests and responses
- Validate database state changes
- Test authentication and authorization
- Performance and load testing

**Test Scenarios:**

- CRUD operations end-to-end
- Validation error handling
- Authentication flows
- Database transaction rollback
- API contract compliance

---

## 7. Development Setup

### 7.1 Prerequisites

- .NET 9.0 SDK
- SQL Server 2022 (or SQL Server Express/LocalDB)
- Docker Desktop
- Visual Studio 2022 17.8+ or VS Code with C# extension

### 7.2 Local Development Setup

```bash
# Clone repository
git clone https://github.com/gauravsiwach/QuickBite-AI-App-FoodMenuManagement.git
cd QuickBite-AI-App-FoodMenuManagement

# Restore dependencies
dotnet restore

# Setup database
dotnet ef database update

# Run application
dotnet run --project QuickBite-AI-App.API

# Run tests
dotnet test
```

### 7.3 Environment Configuration

**Configuration Requirements:**

- Connection string configuration for SQL Server
- Environment-specific settings (Development, Production)
- Logging level configuration per environment
- Swagger enablement for development environments
- CORS configuration for cross-origin requests
- Authentication and authorization settings

---

## 8. Deployment Configuration

### 8.1 Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["QuickBite-AI-App.API/QuickBite-AI-App.API.csproj", "QuickBite-AI-App.API/"]
RUN dotnet restore "QuickBite-AI-App.API/QuickBite-AI-App.API.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/QuickBite-AI-App.API"
RUN dotnet build "QuickBite-AI-App.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "QuickBite-AI-App.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "QuickBite-AI-App.API.dll"]
```

### 8.2 Docker Compose

```yaml
version: "3.8"

services:
  QuickBite-AI-App-api:
    build:
      context: .
      dockerfile: QuickBite-AI-App.API/Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=QuickBite-AI-AppDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true
    depends_on:
      - sqlserver
    networks:
      - QuickBite-AI-App-network

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - QuickBite-AI-App-network

volumes:
  sqlserver_data:

networks:
  QuickBite-AI-App-network:
    driver: bridge
```

---

## 9. Code Standards

### 9.1 Naming Conventions

- **Classes**: PascalCase (`FoodItemService`)
- **Methods**: PascalCase (`GetFoodItemAsync`)
- **Properties**: PascalCase (`Name`, `Price`)
- **Fields**: camelCase with underscore (`_dbContext`)
- **Constants**: PascalCase (`MaxPageSize`)

### 9.2 Code Quality Tools

```xml
<!-- .editorconfig -->
<PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
<PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0" PrivateAssets="all" />
<PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.507" PrivateAssets="all" />
```

### 9.3 Logging Standards

**Structured Logging Requirements:**

- Use structured logging with named parameters
- Implement appropriate log levels for different scenarios
- Include correlation IDs for request tracking
- Avoid logging sensitive information
- Configure different log levels for different environments

**Log Level Guidelines:**

- **Trace**: Detailed trace information for debugging
- **Debug**: Debug information for development
- **Information**: General application flow information
- **Warning**: Warning conditions that should be monitored
- **Error**: Error conditions with exception details
- **Critical**: Critical errors requiring immediate attention

---

## 10. Performance Considerations

### 10.1 Database Optimization

- **Indexes**: On frequently queried columns (category, dietary_tag)
- **Pagination**: Implement efficient pagination using Skip/Take
- **Connection Pooling**: Configure appropriate pool sizes
- **Query Optimization**: Use Include() for related data

### 10.2 API Performance

**Performance Optimization Requirements:**

- Memory caching for frequently accessed data
- Response caching for static content
- GZIP compression for HTTP responses
- Connection pooling for database operations
- Async/await patterns throughout the application

**Caching Strategy:**

- In-memory caching for categories and dietary tags
- Response caching with appropriate cache headers
- Database query result caching where appropriate

### 10.3 Monitoring and Health Checks

**Health Check Requirements:**

- Database connectivity checks
- Custom application health indicators
- Health check endpoints for load balancers
- Application insights integration (optional)
- Performance counters and metrics

**Monitoring Components:**

- SQL Server health verification
- Memory usage monitoring
- Response time tracking
- Error rate monitoring

---

## 11. Next Steps

1. **Project Initialization**: Create .NET 9 project structure
2. **Database Setup**: Configure Entity Framework and SQL Server
3. **Implement Models**: Create entities and DTOs
4. **Service Layer**: Implement business logic with TDD
5. **API Controllers**: Create REST endpoints
6. **Testing**: Comprehensive unit and integration tests
7. **Documentation**: Swagger/OpenAPI setup
8. **Containerization**: Docker configuration
9. **Deployment**: Production deployment setup

---

_This technical documentation provides the foundation for implementing the QuickBite-AI-App Food Menu Management system using modern .NET technologies and best practices._
