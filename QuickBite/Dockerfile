# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory
WORKDIR /app

# Copy the solution file and project files
COPY *.sln ./
COPY QuickBite.AI.App.API/*.csproj ./QuickBite.AI.App.API/
COPY QuickBite.AI.App.Tests/*.csproj ./QuickBite.AI.App.Tests/

# Restore dependencies
RUN dotnet restore

# Copy the entire source code
COPY . .

# Build the application
RUN dotnet build QuickBite.AI.App.API/QuickBite.AI.App.API.csproj -c Release --no-restore

# Run tests
RUN dotnet test QuickBite.AI.App.Tests/QuickBite.AI.App.Tests.csproj -c Release --no-build --verbosity normal

# Publish the application
RUN dotnet publish QuickBite.AI.App.API/QuickBite.AI.App.API.csproj -c Release -o /app/publish --no-build

# Use the official .NET 9 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Create directory for SQLite database with proper permissions
USER root
RUN mkdir -p /app/data && chown -R appuser /app/data
USER appuser

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/api/fooditems || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "QuickBite.AI.App.API.dll"]