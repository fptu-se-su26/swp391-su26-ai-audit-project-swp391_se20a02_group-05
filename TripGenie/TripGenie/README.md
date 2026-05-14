# TripGenie Server

TripGenie is a web application designed to help users plan their trips efficiently. This repository contains the backend server built with .NET 8.

## Technologies Used

- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Language:** C#
- **Documentation:** Swagger/OpenAPI

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation & Running

1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```

2. Navigate to the server directory:
   ```bash
   cd TripGenie/server
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The server will start, and you can access the Swagger UI at `http://localhost:<port>/swagger` (check console for the specific port).

## API Endpoints

- **GET /api/system/status:** Returns the current status of the server.
- **GET /api/system/health:** Returns health check details.
- **GET /api/system/version:** Returns the application version.
- **GET /api/system/info:** Returns system information.
- **GET /api/system/time:** Returns the current server time.

## Project Structure

- `Controllers/`: Contains the API controllers.
- `Program.cs`: The entry point and configuration of the application.
- `appsettings.json`: Configuration settings for different environments.
