# TaskManager API

A clean architecture .NET 8 Web API for managing tasks with CQRS pattern implementation.

## 🏗️ Architecture

This project follows Clean Architecture principles with the following layers:

- **API Layer** (`TaskManager.Api`) - Controllers, middleware, and API configuration
- **Application Layer** (`TaskManager.Application`) - Use cases, commands, queries, and business logic
- **Domain Layer** (`TaskManager.Domain`) - Entities, value objects, and domain rules
- **Infrastructure Layer** (`TaskManager.Infrastructure`) - Data access, repositories, and external services

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- Git

### Installation

1. Clone the repository:
```bash
git clone https://github.com/ezequielcsilva/TaskManager.git
cd TaskManager
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

### Running the Application

#### Option 1: HTTP (Recommended for development)
```bash
cd src/TaskManager.Api
dotnet run --launch-profile http
```

The API will be available at:
- **HTTP**: `http://localhost:5222`

### Swagger Documentation

Once the application is running, access the Swagger UI at:
- **HTTP**: `http://localhost:5222/swagger`

## 📚 API Endpoints

### Base URL
- HTTP: `http://localhost:5222/api/tasks`

### 1. Create Task
**POST** `/api/tasks`

Creates a new task for a user.

**Request Body:**
```json
{
  "title": "Implement JWT authentication",
  "description": "Implement JWT authentication system for the TaskManager API",
  "dueDate": "2024-12-31T23:59:59Z",
  "userId": "123e4567-e89b-12d3-a456-426614174000"
}
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Implement JWT authentication",
  "description": "Implement JWT authentication system for the TaskManager API",
  "createdAt": "2024-01-15T10:30:00Z",
  "dueDate": "2024-12-31T23:59:59Z",
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "isCompleted": false
}
```

### 2. Get User Tasks
**GET** `/api/tasks/{userId}`

Retrieves all tasks for a specific user.

**Parameters:**
- `userId` (Guid) - The user's unique identifier

**Response (200 OK):**
```json
{
  "tasks": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Implement JWT authentication",
      "description": "Implement JWT authentication system for the TaskManager API",
      "createdAt": "2024-01-15T10:30:00Z",
      "dueDate": "2024-12-31T23:59:59Z",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "isCompleted": false
    }
  ]
}
```

### 3. Complete Task
**PUT** `/api/tasks/{id}/complete`

Marks a task as completed.

**Parameters:**
- `id` (Guid) - The task's unique identifier

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Implement JWT authentication",
  "description": "Implement JWT authentication system for the TaskManager API",
  "createdAt": "2024-01-15T10:30:00Z",
  "dueDate": "2024-12-31T23:59:59Z",
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "isCompleted": true
}
```

### 4. Delete Task
**DELETE** `/api/tasks/{id}`

Deletes a task permanently.

**Parameters:**
- `id` (Guid) - The task's unique identifier

**Response (204 No Content)**

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/TaskManager.UnitTests
dotnet test tests/TaskManager.ArchitectureTests
```

### Test Projects

- **Unit Tests** (`TaskManager.UnitTests`) - Unit tests for all layers
- **Architecture Tests** (`TaskManager.ArchitectureTests`) - Architecture compliance tests

## 🛠️ Development

### Project Structure

```
TaskManager/
├── src/
│   ├── TaskManager.Api/           # Web API layer
│   ├── TaskManager.Application/   # Application layer (CQRS)
│   ├── TaskManager.Domain/        # Domain layer
│   └── TaskManager.Infrastructure/ # Infrastructure layer
├── tests/
│   ├── TaskManager.UnitTests/     # Unit tests
│   └── TaskManager.ArchitectureTests/ # Architecture tests
└── README.md
```

### Key Technologies

- **.NET 8** - Framework
- **Entity Framework Core** - ORM (In-Memory Database)
- **FluentValidation** - Input validation
- **Scrutor** - Dependency injection scanning
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework
- **NetArchTest** - Architecture testing

### CQRS Pattern

The application implements Command Query Responsibility Segregation (CQRS):

- **Commands** - Write operations (Create, Update, Delete)
- **Queries** - Read operations (Get, List)
- **Handlers** - Business logic implementation
- **Validators** - Input validation using FluentValidation

## 🔧 Configuration

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to `Development` for local development

### Database

The application uses an in-memory database for development purposes. Data is not persisted between application restarts.

## 📝 API Testing

Use the provided `TaskManager.Api.http` file in VS Code with the REST Client extension, or use tools like Postman, Insomnia, or curl.

### Example Requests

```bash
# Create a task
curl -X POST "http://localhost:5222/api/tasks" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Task",
    "description": "This is a test task",
    "dueDate": "2024-12-31T23:59:59Z",
    "userId": "123e4567-e89b-12d3-a456-426614174000"
  }'

# Get user tasks
curl -X GET "http://localhost:5222/api/tasks/123e4567-e89b-12d3-a456-426614174000"

# Complete a task
curl -X PUT "http://localhost:5222/api/tasks/{taskId}/complete"

# Delete a task
curl -X DELETE "http://localhost:5222/api/tasks/{taskId}"
```

## 🐛 Troubleshooting

### Common Issues

1. **Port already in use**: Change the port in `launchSettings.json`

### Logs

The application uses Serilog for structured logging. Check the console output for detailed error information.

## 📄 License

This project is licensed under the MIT License.
