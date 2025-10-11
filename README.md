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

### Application start
To run the application locally, you need **Docker** installed on your machine.

1. **Clone the repository**
```bash
git clone https://gitlab.fi.muni.cz/xkuchar/pv179-project.git
```
2. **Navigate to the project directory**
```bash
cd pv179-project
```
3. **Database setup**

Open Docker and then run:
```bash
docker compose up -d
```

4. **Build the application**
```bash
dotnet build
```
5. **Switch to the API directory**
```bash
cd API
```
6. **Run the application**
```bash
dotnet run
```
7. **Open your web browser and navigate to**
```
http://localhost:5076/swagger
```

## Technical overview
- ASP.NET Core MVC
- Entity Framework Core with Code First approach
- Serilog for logging
- GitLab CI/CD for continuous integration and delivery


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
The project includes a custom logging middleware built with Serilog.
It records details about every HTTP request and response — including the method, URL, response status code, and how long the request took to process.
The middleware was inspired by [this article](https://nblumhardt.com/2024/04/serilog-net8-0-minimal).

### GitLab CI/CD and Repository Setup
The repository is configured with **GitLab CI/CD** to automate building, testing, and validating code changes.

Key settings include:
- **Protected branches** to ensure that only approved changes can be merged into main development branches.
- **Required reviewers** for merge requests, enforcing code quality and peer review.

## Use case diagram
![Use case diagram](Docs/pv179-usecase.png)
## Data model
![Data model](Docs/pv179-datamodel-vol2.png)
