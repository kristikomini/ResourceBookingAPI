# ResourceBookingAPI

A REST API for booking shared resources (cars, meeting rooms, etc.). It provides
JWT-based authentication, user management, resource and resource-type management,
and booking management with overlap prevention.

## Tech stack

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core 10** (SQL Server provider)
- **AutoMapper** for entity ↔ DTO mapping
- **JWT bearer** authentication
- **Swagger / OpenAPI** for interactive documentation
- **xUnit** + EF Core SQLite (in-memory) for tests

## Project structure

```
ResourceBooking/                 # Web API project
├── Controller/                  # API controllers (Auth, Users, Bookings, Resources, ResourceTypes)
├── Data/                        # EF Core DbContext
├── Dto/                         # Request/response DTOs
├── Exceptions/                  # Domain exceptions (e.g. ResourceAlreadyBookedException)
├── Helpers/                     # AutoMapper profiles
├── Interface/                   # Repository interfaces
├── Middleware/                  # Global exception handler (ProblemDetails)
├── Migrations/                  # EF Core migrations
├── Models/                      # Entity models
├── Repository/                  # Repository implementations
├── Services/                    # TokenService (JWT generation)
├── Seed.cs                      # Optional database seeding
└── Program.cs                   # Application startup / DI wiring
ResourceBooking.Tests/           # xUnit test project
```

## Getting started

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full) — or update the connection string for your provider

### Configuration

`appsettings.json` ships with **placeholder** values only. Never commit real
secrets. Provide the JWT signing key, issuer/audience, and the connection string
through [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
(local development) or environment variables (deployment):

```bash
cd ResourceBooking
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<a strong random key of at least 32 characters>"
dotnet user-secrets set "Jwt:Issuer" "ResourceBookingApi"
dotnet user-secrets set "Jwt:Audience" "ResourceBookingApiClients"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string>"
```

Environment variable equivalents (note the double underscore):

```bash
export Jwt__Key="<a strong random key of at least 32 characters>"
export ConnectionStrings__DefaultConnection="<your connection string>"
```

### Database

Apply the EF Core migrations to create the schema:

```bash
dotnet tool install --global dotnet-ef      # once, if not already installed
dotnet ef database update --project ResourceBooking
```

Optionally seed sample data (two users, resource types, resources, and bookings):

```bash
dotnet run --project ResourceBooking seeddata
```

> Passwords are hashed with ASP.NET Core Identity's `PasswordHasher` (PBKDF2 with
> a per-user salt). The seeded demo users are `john.doe@example.com` and
> `jane.doe@example.com`, both with password `password123`.

### Run

```bash
dotnet run --project ResourceBooking
```

In development, Swagger UI is available at `https://localhost:7108/swagger`
(see `Properties/launchSettings.json` for the exact ports).

## Authentication

Most endpoints require a valid JWT. The typical flow:

1. `POST /api/users` — register a new user (anonymous).
2. `POST /api/auth/login` — exchange email + password for a JWT.
3. Send the token on subsequent requests: `Authorization: Bearer <token>`.

## Testing

```bash
dotnet test
```

Tests run against an in-memory SQLite database, so no SQL Server instance is
required. Continuous integration builds and tests every push and pull request to
`main` (see `.github/workflows/ci.yml`).

## Contributing

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/my-change`).
3. Commit your changes.
4. Push the branch and open a pull request.

## Contact

Kristi Komini — kristi.komini@studenti.unicam.it
