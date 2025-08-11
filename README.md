# Task Management App

A modern, high-performance task management application built with ASP.NET Core 9.0, featuring vertical slice architecture, clean architecture, advanced data projection.

## 🚀 Features

- **Multiple Authentication Providers**: Support for credentials-based and Google OAuth authentication
- **Security**: Argon2 password hashing with configurable memory usage
- **API Versioning**: Versioned REST API with OpenAPI documentation
- **Field Projection**: Dynamic field selection for optimized API responses
- **Database Migrations**: Entity Framework Core with PostgreSQL
- **Error Handling**: Comprehensive error handling with structured problem details
- **Validation**: FluentValidation with custom error codes
- **Type Safety**: Strongly-typed entity IDs with custom converters

## 🏗️ Architecture

The project follows a hybrid architecture approach that combines **Vertical Slice Architecture** with traditional **Clean Architecture** layers. While maintaining the layered separation of Clean Architecture, the organization within each layer follows vertical slice principles - grouping by features rather than technical concerns.

### Architecture Overview

```
├── WebApp.Api/                 # Main API Host - Configuration, middleware, program entry
├── WebApp.Api.Users/           # Users Feature Slice - All user-related API logic
├── WebApp.Api.Common/          # Shared API utilities and cross-cutting concerns
├── WebApp.Domain/              # Domain Layer - Entities, value objects, domain logic
└── WebApp.Infrastructure/      # Infrastructure Layer - Data access, external services
```

### Vertical Slice Benefits

- **Feature Cohesion**: All code related to a specific feature (Users, Tasks, etc.) is grouped together
- **Independent Development**: Teams can work on different features with minimal conflicts
- **Easier Testing**: Each slice can be tested independently
- **Simplified Navigation**: Finding feature-related code is intuitive
- **Reduced Coupling**: Features are loosely coupled, making changes safer

### Clean Architecture Foundation

- **Domain Layer**: Contains business entities and rules, independent of external concerns
- **Infrastructure Layer**: Handles data persistence, external APIs, and technical implementations
- **Presentation Layer**: Split into feature-based API projects for better organization

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0 (.NET 9)
- **Database**: PostgreSQL with Entity Framework Core 9.0
- **Authentication**: Multi-provider (Credentials + Google OAuth)
- **Security**: Argon2 password hashing (Geralt library)
- **Validation**: FluentValidation
- **API Documentation**: OpenAPI/Swagger
- **ID Encoding**: Sqids for obfuscated public IDs
- **Time**: NodaTime for robust datetime handling
- **Architecture**: Clean Architecture with CQRS patterns

## 📋 Prerequisites

- .NET 9.0 SDK
- PostgreSQL 13+ or Docker
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd task-management-app
```

### 2. Setup Database

#### Using Docker (Recommended)

```bash
cd api
docker-compose up -d
```

This will start a PostgreSQL container with the following credentials:
- **Database**: webapp
- **Username**: webapp
- **Password**: 123456
- **Port**: 5432

#### Manual PostgreSQL Setup

If you prefer to use an existing PostgreSQL installation, update the connection string in `appsettings.Development.json`.

### 3. Configure Application Settings

Create `appsettings.Development.json` in the `WebApp.Api` folder:

```json
{
  "Data": {
    "ConnectionString": "Host=localhost;Port=5432;Database=webapp;Username=webapp;Password=123456"
  },
  "NumberEncoder": {
    "Alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
  }
}
```

### 4. Run Database Migrations

```bash
cd api/WebApp.Api
dotnet ef database update
```

### 5. Start the Application

```bash
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5041
- **HTTPS**: https://localhost:7015
- **OpenAPI**: https://localhost:7015/openapi/v1.json (in development)

### Authentication Providers

- **Credentials**: Traditional username/password
- **Google**: OAuth 2.0 integration

## 🏛️ Project Structure

### Vertical Slice Organization

Each feature is organized as a vertical slice containing all the layers needed for that specific functionality:

#### Feature Slices (Presentation Layer)
- **WebApp.Api.Users/**: Complete user management feature
  - `V1/CreateUser.cs` - User creation endpoint and logic
  - `V1/GetUserById.cs` - User retrieval endpoint and logic
  - `Extensions/WebApplicationExtensions.cs` - API routing configuration
- **WebApp.Api.Common/**: Shared cross-cutting concerns
  - `Projection/` - Field projection utilities
  - `Hashing/` - Password hashing services
  - `Codecs/` - ID encoding/decoding
  - `Http/` - Common HTTP utilities and error handling

#### Shared Layers
- **WebApp.Domain/**: Business entities and domain logic
  - `Entities/` - Domain entities (User, UserAuth, etc.)
  - `Constants/` - Domain constants and enums
- **WebApp.Infrastructure/**: Data access and external services
  - `Data/` - Entity Framework configuration and DbContext
  - `Data/Configs/` - Entity configurations organized by feature

### Core Components

- **Entity IDs**: Strongly-typed identifiers (UserId, UserAuthId)
- **Soft Delete**: Automatic filtering of deleted entities
- **HiLo Sequences**: High-performance ID generation
- **Snake Case**: Database naming convention
- **Value Converters**: Automatic ID and enum conversion

### Key Features

- **Field Projection**: Request only needed fields (`?fields=name,id`)
- **Exception Handling**: Structured error responses with problem details
- **Validation**: Custom error codes and multilingual support
- **Number Encoding**: Obfuscated public IDs using Sqids
- **JSON Configuration**: Optimized serialization with NodaTime support

## 🧪 Development

### Build the Solution

```bash
cd api
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Add Migration

```bash
cd WebApp.Api
dotnet ef migrations add <MigrationName>
```

## 🐳 Docker Support

The project includes Docker Compose for easy PostgreSQL setup:

```yaml
services:
  db:
    image: postgres:alpine
    environment:
      POSTGRES_DB: webapp
      POSTGRES_USER: webapp
      POSTGRES_PASSWORD: 123456
    ports:
      - "5432:5432"
```

## 🔧 Configuration

### Key Configuration Sections

- **Data**: Database connection and migration settings
- **NumberEncoder**: ID obfuscation settings
- **Logging**: Application logging configuration

### Environment-Specific Settings

- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development overrides
- `appsettings.Production.json`: Production settings

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with ASP.NET Core 9.0, pure Minimal APIs
- Uses Entity Framework Core for data access
- Inspired by vertical slice architecture and clean architecture
- Security provided by Argon2 hashing algorithm