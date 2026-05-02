# AffirmationsApp

ASP.NET Core MVC app for managing affirmations and categories.

## Tech Stack

- .NET 9
- ASP.NET Core MVC
- Entity Framework Core 9
- SQL Server

## Prerequisites

- .NET SDK 9.0+
- SQL Server instance (local, Docker, or hosted)

## Configuration

The app uses a SQL Server connection string from `appsettings.json`:

`ConnectionStrings:DefaultConnection`

Current default:

`Server=localhost,11433;Database=AffirmationsDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;`

Update this value to match your local environment.

## Run Locally

1. Restore packages:
   `dotnet restore`
2. Build:
   `dotnet build`
3. Run:
   `dotnet run`

Default route starts at:

`/Affirmations/Index`

## Project Structure

- `Controllers/` MVC controllers
- `Models/` domain models
- `Data/` EF Core DbContext
- `Views/` Razor views
- `wwwroot/` static files

