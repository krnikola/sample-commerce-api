# SampleCommerce API

Status: portfolio project / actively maintained

SampleCommerce API is a sample ASP.NET Core Web API project for user authentication, product browsing, favorites, and basket management.

The application combines local persistence with an external product source:
- user-related data, favorites, and basket data are stored in SQL Server
- product data is fetched from the DummyJSON API
- product responses support pagination, sorting, and in-memory caching

## Features

### Authentication
- User registration
- User login
- Get currently authenticated user

### Products
- Get all products
- Get product by id
- Pagination support (`skip`, `limit`)
- Sorting support (`sortBy`, `sortDir`)
- In-memory caching for product list and single product fetches

### Favorites
- Add product to favorites
- Remove product from favorites
- Get current user's favorites

### Basket
- Add product to basket
- Remove product from basket
- Get current user's basket
- Product existence validation before adding to basket

## Tech Stack
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT authentication
- Swagger / OpenAPI
- DummyJSON external API
- In-memory caching (`IMemoryCache`)

## Architecture

The solution is organized into multiple projects:
- `SampleCommerce.Api` – API layer, controllers, startup, Swagger configuration
- `SampleCommerce.Application` – application-level service registration
- `SampleCommerce.Domain` – domain models
- `SampleCommerce.Infrastructure` – persistence, external API client, caching, stores, migrations

## Running the Project

### Prerequisites
- .NET 9 SDK
- SQL Server

### Configuration
Update connection string and JWT settings in:
- `appsettings.json`
- `appsettings.Development.json`

Example connection string:

    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=SampleCommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
    }

### Database
Apply migrations before first run.

### Start the API

    dotnet run --project SampleCommerce.Api

After startup, Swagger UI should be available at:

    https://localhost:<port>/swagger

## Example Request

    /api/Product?skip=0&limit=10&sortBy=price&sortDir=desc

## Notes
- Product catalog data is fetched from DummyJSON and is not stored locally.
- Favorites and basket data are persisted in SQL Server.
- Product list sorting is currently applied to the fetched result set for the requested page.
- Repeated product requests are cached temporarily to reduce calls to the external API.

## Possible Future Improvements
- global sorting before pagination
- product search and filtering
- automated tests
- cache invalidation strategy
- Docker support