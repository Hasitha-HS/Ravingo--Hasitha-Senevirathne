# Ravigno Branch Stock-Transfer System

An enterprise-grade ASP.NET Core Web API with a React frontend built for managing and tracking branch-to-branch inventory transfers, ensuring transaction safety, and preventing negative stock levels.

---

## Project Structure
- Ravigno.StockTransfer.Api/ - ASP.NET Core Web API (Backend)
- FE/ - React + Vite Application (Frontend)

---

## Prerequisites
- .NET 8 SDK or later
- Node.js (v18+ recommended) & npm
- PostgreSQL instance running locally or remotely

---

## 1. Backend Setup (.NET Web API)

### Configuration (appsettings.json)
Ensure your PostgreSQL connection string in appsettings.json is correctly set:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ravigno_db;Username=postgres;Password=postgres"
  }
}

### Running the Backend
1. Open a terminal in the root project directory (where the .sln or .csproj file resides).
2. Restore NuGet packages and start the API using PowerShell:
   dotnet restore
   dotnet run
3. The database, tables, and seed data (1 Central Warehouse, 3 Branches, 5 Products, initial stock) are automatically created on startup via EF Core EnsureCreated().
4. Open your browser and navigate to Swagger UI: https://localhost:7048/swagger

---

## 2. Frontend Setup (React + Vite)

### Running the Frontend
1. Navigate to the FE directory:
   cd FE
2. Install dependencies:
   npm install
   npm install axios
3. Start the Vite development server:
   npm run dev
4. Open the application in your browser at http://localhost:5173.

---

## Core API Endpoints

- GET /api/branches - Fetch all branches and warehouses.
- GET /api/branches/{id}/stock - Fetch inventory stock levels for a specific branch.
- GET /api/transfers - List all stock transfers (supports filtering by branchId and status).
- POST /api/transfers - Request a new stock transfer between branches.
- POST /api/transfers/{id}/dispatch - Dispatch requested transfer (deducts stock transactionally, prevents negative inventory).
- POST /api/transfers/{id}/receive - Receive dispatched transfer (adds stock to destination branch).
- POST /api/transfers/{id}/cancel - Cancel a transfer while in Requested status.
