# PV179 - project
### Authors
- Josef Kuchař
- Jiří Štípek
- Andrej Nešpor

### Description
This project is a web platform inspired by Herohero, where creators can share video content and connect with their audience.
Users can:

- Create profiles and upload videos
- Subscribe to their favorite creators
- Build and manage playlists
- Comment on videos

The application is built with ASP.NET Core MVC and uses a multi-layered architecture to keep the codebase clean, maintainable, and easy to extend.

### Prerequisites
Required:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet) (8.0.x)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) with Docker Compose v2
- [Git](https://git-scm.com/downloads)

Optional:
- Visual Studio 2022 or VS Code
- EF Core CLI: `dotnet tool install -g dotnet-ef`
- PostgreSQL client (psql) for debugging
- Postman (for testing API endpoints)

### Application start
To run the application locally, you need Docker installed.

1. **Clone the repository**
```bash
git clone https://gitlab.fi.muni.cz/xkuchar/pv179-project.git
cd pv179-project
```

2. **Start the database**
```bash
docker compose up -d
```
This starts a PostgreSQL instance defined in `docker-compose.yml`.

3. **Build the solution**
```bash
dotnet build
```

4. **Run the solution**
```bash
dotnet run
```
On first start, the API will:
- Apply EF Core migrations automatically
- Seed sample data in Development environment

5. **Open your browser** (replace the port if different in console output)
```
http://localhost:5076/swagger
```

## Technical overview
- .NET 8, C#
- ASP.NET Core:
  - API project with REST controllers and Swagger
  - MVC project with Razor Pages Identity UI
- Entity Framework Core (Code First) with PostgreSQL (via Docker Compose)
- Serilog logging: console sink and PostgreSQL sink (configured in `API/appsettings.json`) + `UseSerilogRequestLogging()`
- EF Core audit logging to `AuditLogs` table (entity create/update/delete)
- Layered architecture: DAL, Infra, Business, Common, API, MVC
- GitLab CI/CD for automated build/test validation

### Audit logger
Entity changes are recorded into an `AuditLogs` table automatically during `SaveChanges`/`SaveChangesAsync`.
- Scope: currently logs `Video` entity create/update/delete operations
- Captured fields: `UserId`, `Table`, `EntityId`, `Action` (`Create|Update|Delete`), `CreatedAt`, `UpdatedAt`
- Source: appended by `AppendAuditLogs()` in `DAL/Data/AppDbContext`
- Migrations: table is created by EF migrations and applied on API startup

This can be extended to other entities by adjusting the logic in `AppendAuditLogs()`.

### Identity authentication
Authentication is handled by ASP.NET Core Identity with the custom `User` entity stored via EF Core in PostgreSQL, exposed through Razor Pages UI.
- Registration in MVC: `AddIdentity<User, IdentityRole>()` + `AddEntityFrameworkStores<AppDbContext>()` + `AddDefaultTokenProviders()`
- Email sender: `IEmailSender` registered (development stub in `MVC.Services.EmailSender`)
- Docs: https://learn.microsoft.com/aspnet/core/security/authentication/identity

### Architecture
The project is divided into several layers, each with its own clear purpose:

1. **Data Access Layer (DAL)**
handles everything related to the database — defining entities, managing migrations, and implementing repositories for data operations.

2. **Business Layer (BL)**
contains the application’s core logic. This is where the main features are implemented — such as managing subscriptions, playlists, or comments.

3. **Application Layer (API)**
exposes REST endpoints that connect the MVC frontend (or external services) with the Business layer.

4. **Model-View-Controller Layer (MVC)**
provides the user interface and handles web requests. It includes controllers and view models that define how data is displayed and interacted with.

Each layer communicates only with the one directly below it, ensuring a clean separation of responsibilities.

### Logging middleware
The API configures Serilog in `API/Program.cs` (console sink by default). A request logging middleware (`UseSerilogRequestLogging`) emits structured logs for each HTTP request (method, path, status code, duration).

Request logs themselves are not written directly to the database. Persistent logging to the database is achieved through the EF Core audit mechanism (see the Audit logger section) which stores entity change events in the `AuditLogs` table.

To also persist request logs into PostgreSQL you could add a Serilog PostgreSQL sink (e.g. `Serilog.Sinks.PostgreSQL`) and register it in the initial Serilog configuration.

### GitLab CI/CD and Repository Setup
The repository is configured with **GitLab CI/CD** for automated build/test validation.

Key settings include:
- **Protected branches** to ensure that only approved changes can be merged into main development branches.
- **Required reviewers** for merge requests, enforcing code quality and peer review.

## Use case diagram
![Use case diagram](Docs/pv179-usecase.png)
## Data model
![Data model](Docs/pv179-datamodel-vol2.png)
