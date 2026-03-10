# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Pesca Cisne** — ASP.NET Core 9.0 MVC e-commerce catalog for frozen fish products. Uses PostgreSQL with Entity Framework Core and ASP.NET Identity for authentication.

## Common Commands

```bash
# Build
dotnet build

# Run (HTTP: localhost:5244, HTTPS: localhost:7234)
dotnet run --project FrozenFishCatalog

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project FrozenFishCatalog

# Apply migrations manually (also runs automatically on startup)
dotnet ef database update --project FrozenFishCatalog
```

No test project exists in the solution.

## Architecture

**MVC with EF Core** — standard three-layer structure inside `FrozenFishCatalog/`:

- **`Data/ApplicationDbContext.cs`** — EF Core context extending `IdentityDbContext<ApplicationUser>`. Configures decimal precision (10,2) for price fields and cascade deletes for user-owned entities.
- **`Data/DbSeeder.cs`** — Seeds 4 categories, 16 products, and 3 `ProductWeight` entries per product on startup. Called in `Program.cs` after `context.Database.Migrate()`.
- **`Models/`** — Domain models: `Product` → many `ProductWeight` (size/price variants) → belongs to `Category`. `CartItem` and `Order`/`OrderItem` are tied to `ApplicationUser`.
- **`ViewModels/`** — DTOs between controllers and views. Computed properties (e.g., totals) live here, not in models.
- **`Controllers/`** — `CartController` and `CheckoutController` are `[Authorize]`-gated. Checkout performs mock payment processing and creates `Order`/`OrderItem` records.

## Database

The app resolves the connection in this priority order (see `Program.cs`):
1. `DATABASE_URL` environment variable — used in production (Heroku). Accepts the `postgres://` URI format that Heroku provides.
2. `ConnectionStrings:DefaultConnection` in `appsettings.json` — fallback for local development.

Migrations live in `FrozenFishCatalog/Migrations/`. The app auto-migrates and seeds on every startup — seeding checks for existing data before inserting.

## Key Conventions

- Products have multiple `ProductWeight` entries (1kg, 2kg, 3kg) — price and stock are per weight variant, not per product.
- Identity password rules: 6+ chars, requires digit, upper/lowercase, unique email (configured in `Program.cs`).
- Authentication cookie paths are customized (login: `/Account/Login`, access denied: `/Account/AccessDenied`).
- Static libraries (jQuery, Bootstrap, jQuery Validation) are vendored under `wwwroot/lib/`.
