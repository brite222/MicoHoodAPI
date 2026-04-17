# MicoHood API

A RESTful backend API for **MicoHood**, a hyperlocal community platform.  
Built with **ASP.NET Core 8**, **PostgreSQL**, **Entity Framework Core**, and **JWT Authentication**.

---

## Tech Stack

- ASP.NET Core 8 Web API
- PostgreSQL + Entity Framework Core (Npgsql)
- JWT Bearer Authentication
- Repository Pattern + SOLID Principles
- Swagger / OpenAPI
- Docker
- Deployed on Render

---

## Features

- User registration and login with JWT
- Create and retrieve community posts
- Like / Unlike posts (toggle)
- Filter posts by location (e.g. Yaba, Lekki)
- Pagination on all post listings
- Global error handling middleware
- Swagger UI with JWT authorization support

---

## Getting Started (Local)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### 1. Clone the repo
```bash
git clone https://github.com/your-username/micohood-api.git
cd micohood-api
```

### 2. Update appsettings.json
Edit `appsettings.json` and set your local PostgreSQL connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=micohood;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-at-least-32-characters",
    "Issuer": "MicoHoodAPI",
    "Audience": "MicoHoodClient"
  }
}
```

### 3. Run migrations
```bash
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run
```

Swagger UI will be available at: `http://localhost:5000`

---

## Running with Docker

```bash
docker build -t micohood-api .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="your_postgres_url" \
  -e Jwt__Secret="your_secret" \
  -e Jwt__Issuer="MicoHoodAPI" \
  -e Jwt__Audience="MicoHoodClient" \
  micohood-api
```

---

## API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Posts
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/posts` | Optional | Get all posts |
| GET | `/api/posts/{id}` | Optional | Get post by ID |
| POST | `/api/posts` | Required | Create a new post |
| POST | `/api/posts/{id}/like` | Required | Toggle like/unlike |

### Query Parameters for GET /api/posts
| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| location | string | Filter by area | `?location=Lekki` |
| page | int | Page number (default: 1) | `?page=2` |
| pageSize | int | Items per page (default: 10, max: 50) | `?pageSize=5` |

**Example:**