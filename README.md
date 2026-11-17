# Buyora

Buyora is a bilingual e-commerce web application built with ASP.NET Core MVC. It supports Arabic (Egypt) and English, customer accounts, product browsing, a shopping cart, order management, and an admin area for managing products, categories, orders, and users.

## Technology

- ASP.NET Core MVC on .NET 10
- Entity Framework Core 10 with SQL Server
- ASP.NET Core Identity
- Layered solution: `Buyora` (web), `BuyoraBusiness`, `BuyoraDataAccess`, `BuyoraModels`, and `BuyoraUtility`

## Requirements

- .NET 10 SDK
- SQL Server (LocalDB, Express, or another SQL Server instance)
- Git

## Getting started

1. Clone the repository and open `Buyora.slnx` in Visual Studio, or use the .NET CLI.
2. Configure the database connection string and any authentication or email settings as described below.
3. From the repository root, restore and run the web project:

   ```bash
   dotnet restore Buyora.slnx
   dotnet run --project Buyora/Buyora.csproj
   ```

4. Open the local URL printed by `dotnet run`. The application defaults to Arabic; use the language selector to switch to English.

The application runs its database initializer at startup. Keep the checked-in EF Core migrations in sync with model changes. To apply migrations explicitly, install the EF Core CLI tool if needed and run:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project BuyoraDataAccess/BuyoraDataAccess.csproj --startup-project Buyora/Buyora.csproj
```

## Configuration

Do not commit real passwords, API keys, or OAuth client secrets. For local development, use .NET User Secrets from the web project:

```bash
cd Buyora
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=Buyora;Trusted_Connection=True;TrustServerCertificate=True"
```

Use the connection string key and any other settings expected by the application configuration in the dependency injection setup. User Secrets are for development only. In deployed environments, provide secrets through environment variables or the hosting provider's secret store. For example, nested configuration keys use double underscores in environment variable names (`ConnectionStrings__DefaultConnection`).

The application also supports configuration through `appsettings*.json`; keep those files free of credentials. `App:BaseUrl` should be set to the public application URL in the target environment.

## Project layout

| Project | Responsibility |
| --- | --- |
| `Buyora` | ASP.NET Core MVC web application, areas, views, static assets, and localization |
| `BuyoraBusiness` | Application services and business logic |
| `BuyoraDataAccess` | EF Core context, repositories, unit of work, migrations, and initialization |
| `BuyoraModels` | Domain models and DTOs |
| `BuyoraUtility` | Shared constants and utilities |

## Features

- Customer storefront, product details, shopping cart, and order history
- Admin dashboard and product, category, order, and user management
- ASP.NET Core Identity authentication and role management
- Arabic and English localization
- Product image uploads

## License

No license file is currently included. All rights are reserved unless the repository owner specifies otherwise.
