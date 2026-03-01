# ShiftMaster

Enterprise-grade workforce planning and shift management platform.

## Architecture

- **Frontend**: Angular 18 + Angular Material
- **Backend**: .NET 8 Web API microservices
- **API Gateway**: YARP reverse proxy
- **Auth**: JWT with ASP.NET Core Identity
- **Database**: PostgreSQL (one per microservice)
- **Messaging**: RabbitMQ

## Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- PostgreSQL (or use Docker for infra only)
- Docker (optional)

### 1. Run Infrastructure (PostgreSQL + RabbitMQ)

```bash
docker run -d --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16-alpine
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management-alpine
```

### 2. Create Databases

```sql
CREATE DATABASE shiftmaster_auth;
CREATE DATABASE shiftmaster_employee;
CREATE DATABASE shiftmaster_planning;
CREATE DATABASE shiftmaster_absence;
```

### 3. Run Backend Services

```bash
cd src
dotnet run --project Gateway/ShiftMaster.Gateway        # Port 5000
dotnet run --project Services/Auth/ShiftMaster.Auth.API       # Port 5001
dotnet run --project Services/Employee/ShiftMaster.Employee.API   # Port 5002
dotnet run --project Services/Planning/ShiftMaster.Planning.API  # Port 5003
dotnet run --project Services/Absence/ShiftMaster.Absence.API    # Port 5004
dotnet run --project Services/Analytics/ShiftMaster.Analytics.API # Port 5005
dotnet run --project Services/Reporting/ShiftMaster.Reporting.API # Port 5006
dotnet run --project Services/Notification/ShiftMaster.Notification.API # Port 5007
```

Or run each in a separate terminal. The Gateway routes all `/api/*` to the appropriate service.

### 4. Run Frontend

```bash
cd frontend
npm install
npm start
```

Open http://localhost:4200

### 5. Login

| Role    | Email                  | Password    |
|---------|------------------------|-------------|
| Admin   | admin@shiftmaster.com  | Admin123!   |
| Manager | manager@shiftmaster.com| Manager123! |
| Employee| employee@shiftmaster.com| Employee123! |

## Docker (Full Stack)

```bash
docker-compose up -d
```

Gateway: http://localhost:5000  
Frontend: run `npm start` in frontend/ (or add to docker-compose)

## Project Structure

```
src/
├── ShiftMaster.sln
├── Shared/ShiftMaster.Shared          # DTOs, constants
├── Gateway/ShiftMaster.Gateway        # YARP API Gateway
└── Services/
    ├── Auth/ShiftMaster.Auth.API
    ├── Employee/ShiftMaster.Employee.API
    ├── Planning/ShiftMaster.Planning.API
    ├── Absence/ShiftMaster.Absence.API
    ├── Analytics/ShiftMaster.Analytics.API
    ├── Reporting/ShiftMaster.Reporting.API
    └── Notification/ShiftMaster.Notification.API

frontend/                              # Angular SPA
```

## Features

- **Authentication**: JWT, role-based (Admin, Manager, Employee)
- **Dashboard**: KPIs, heatmap, alerts (Manager) | My planning, equity score (Employee)
- **Employees**: CRUD, contract type, seniority, skills, availability
- **Planning**: Intelligent generation (4 shifts A-D), fair rotation, equity, simulation mode
- **Leaves**: Request, approval workflow
- **Analytics**: Team performance, coverage, rotation rate
- **Reporting**: PDF, Excel, audit logs

## API Documentation

Swagger available at each service when running in Development:
- Auth: http://localhost:5001/swagger
- Employee: http://localhost:5002/swagger
- Planning: http://localhost:5003/swagger
- etc.
