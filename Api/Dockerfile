# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY partnerMesh.slnx ./
COPY Api/Api.csproj ./Api/
COPY Application/Application.csproj ./Application/
COPY Domain/Domain.csproj ./Domain/
COPY Infraestructure/Infraestructure.csproj ./Infraestructure/

# Restore packages
RUN dotnet restore Api/Api.csproj

# Copy source code
COPY Api/ ./Api/
COPY Application/ ./Application/
COPY Domain/ ./Domain/
COPY Infraestructure/ ./Infraestructure/

# Build and publish
WORKDIR /src/Api
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQL Server tools for health checks
RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create health check endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl --fail http://localhost:5000/health || exit 1

# Expose port
EXPOSE 5000

# Set entry point
ENTRYPOINT ["dotnet", "Api.dll"]