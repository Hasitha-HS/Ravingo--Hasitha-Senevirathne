Markdown
# Ravigno Branch Stock-Transfer System

An enterprise-grade ASP.NET Core Web API built for managing and tracking branch-to-branch inventory transfers securely, preventing negative stock, and ensuring transaction safety.

---

## Prerequisites
- **.NET 8 SDK** or later installed on your machine.
- **PostgreSQL** installed locally or accessible via connection string.

---

## Required NuGet Packages
The project relies on the following NuGet packages (automatically loaded via `.csproj`, or can be installed via CLI):
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0"/>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0"/>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0"/>
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0"/>
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0"/>
</ItemGroup>
If packages are missing, restore them using:

PowerShell
dotnet restore
Configuration (appsettings.json)
Ensure your PostgreSQL connection string is correctly configured in appsettings.json:

JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ravigno_db;Username=postgres;Password=postgres"
  }
}
How to Run
Open a terminal inside the project root folder.

Run the application:

PowerShell
dotnet run
The app will automatically create the database and seed required initial data (1 Central Warehouse, 3 Branches, 5 Products, and starting stock inventories) on startup using EF Core EnsureCreated().

Open your browser and navigate to Swagger UI to test the endpoints interactively:https://localhost:xxxx/swagger/index.html

API Endpoints Overview
GET /api/branches - Retrieve all branches and warehouses.

GET /api/branches/{id}/stock - View current stock levels for a specific branch.

GET /api/transfers - List all stock transfers (supports filtering by branchId and status).

POST /api/transfers - Request a new stock transfer between different branches.

POST /api/transfers/{id}/dispatch - Dispatch a transfer (deducts stock from sender safely using transactions, rejecting negative stock).

POST /api/transfers/{id}/receive - Receive a transfer (adds stock to receiver branch).

POST /api/transfers/{id}/cancel - Cancel a transfer that is still in Requested status.