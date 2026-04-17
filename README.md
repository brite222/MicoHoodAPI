# MicoHood API

A RESTful backend API for MicoHood — a hyperlocal community platform. Built with **ASP.NET Core 8**, **Entity Framework Core**, **PostgreSQL**, and **JWT authentication**.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (via Npgsql) |
| Auth | JWT Bearer Tokens |
| Docs | Swagger / OpenAPI |
| Deployment | Render (Docker) |

---

## Architecture

The project follows a clean layered architecture with the **Repository Pattern** and **SOLID principles**:

```
MicoHoodApi/
├── Controllers/        # HTTP layer — routes & request/response handling
├── Services/           # Business logic layer
├── Repositories/       # Data access layer (Repository Pattern)
├── Interfaces/         # Abstractions (Dependency Inversion Principle)
├── Entities/           # EF Core models / database tables
├── DTOs/               # Data Transfer Objects (request/response shapes)
├── Data/               # AppDbContext & EF configuration
├── Middleware/         # Global exception handling
└── Helpers/            # Utility classes (e.g. JWT claims extraction)
```

---

## Features

- **User Authentication** — Register & login with JWT tokens
- **Post CRUD** — Create posts, get all, get by ID
- **Like / Unlike** — Toggle like on a post (authenticated users only)
- **Location Filtering** — Filter posts by location (e.g. Yaba, Lekki)
- **Pagination** — All list endpoints support page & pageSize
- **Swagger UI** — Interactive API docs with JWT auth support
- **Global Error Handling** — Consistent error responses via middleware
- **Input Validation** — Data annotations on all DTOs

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL running locally or a hosted instance

### 1. Clone the repo

```bash
git clone https://github.com/YOUR_USERNAME/micohood-api.git
cd micohood-api
```

### 2. Configure environment

Update `appsettings.json` (or set environment variables):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=micohood_db;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "MicoHoodApi",
    "Audience": "MicoHoodClient"
  }
}
```

> ⚠️ Never commit real secrets. Use environment variables in production.

### 3. Run migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Migrations auto-run on startup too via `db.Database.Migrate()`.

### 4. Run the API

```bash
dotnet run
```

Swagger UI will be available at: `http://localhost:5000`

---

## API Reference

### Authentication

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login and get JWT token |

#### Register
```json
POST /api/auth/register
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "securepassword123"
}
```

#### Login
```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "securepassword123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "username": "john_doe",
    "email": "john@example.com"
  }
}
```

---

### Posts

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/posts` | Optional | Get all posts (paginated, filterable) |
| GET | `/api/posts/{id}` | Optional | Get post by ID |
| POST | `/api/posts` | **Required** | Create a new post |
| POST | `/api/posts/{id}/like` | **Required** | Toggle like / unlike |

#### Get All Posts (with filters)
```
GET /api/posts?page=1&pageSize=10&location=Yaba
```

**Response:**
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "items": [...],
    "totalCount": 42,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

#### Create Post
```json
POST /api/posts
Authorization: Bearer <token>

{
  "title": "Street food in Yaba",
  "content": "Just discovered the best suya spot near UI...",
  "location": "Yaba"
}
```

#### Toggle Like
```
POST /api/posts/{id}/like
Authorization: Bearer <token>
```

Calling this endpoint on a post you haven't liked → **likes** it.  
Calling it again → **unlikes** it.

---

## Using Swagger UI

1. Open `http://localhost:5000` in your browser
2. Use `POST /api/auth/login` to get a token
3. Click the **Authorize 🔒** button at the top right
4. Enter your token (just the token, no "Bearer" prefix needed)
5. All protected endpoints are now unlocked

---

## Deployment on Render

### Environment Variables (set in Render dashboard)

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | Full PostgreSQL connection string |
| `Jwt__Key` | Secret key (min 32 chars) |
| `Jwt__Issuer` | `MicoHoodApi` |
| `Jwt__Audience` | `MicoHoodClient` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

### Steps

1. Push code to GitHub
2. Create a new **Web Service** on Render
3. Connect your GitHub repository
4. Set **Environment** to `Docker`
5. Add all environment variables above
6. Create a **PostgreSQL** database on Render and use the connection string
7. Deploy 🚀

---

## SOLID Principles Applied

| Principle | Implementation |
|-----------|---------------|
| **S**ingle Responsibility | Each class has one job: Controllers handle HTTP, Services handle logic, Repositories handle data |
| **O**pen/Closed | New features extend via new implementations, not modifying existing ones |
| **L**iskov Substitution | All repository/service implementations are interchangeable with their interfaces |
| **I**nterface Segregation | Separate interfaces: `IUserRepository`, `IPostRepository`, `IPostLikeRepository`, `IAuthService`, `IPostService`, `IJwtService` |
| **D**ependency Inversion | Controllers depend on `IPostService`, not `PostService` directly — wired via DI in `Program.cs` |

---

## License

MIT
