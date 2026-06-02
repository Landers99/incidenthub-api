# IncidentHub API

A production-style ASP.NET Core backend project focused on backend debugging, API reliability, authentication issues, database performance, Docker deployment, and observability.

## Authentication

IncidentHub uses JWT authentication with role-based authorization.

Supported auth endpoints:

```http
POST /api/auth/register
POST /api/auth/login
GET /api/auth/me
```

Roles:

- User: can create and view incidents
- Admin: can delete incidents and perform administrative actions

Security notes:

- Passowrds are hashed using BCrypt
- JWTs include user ID, email, and role claims
- Incident ownership is derived from the authenticated user token

## Query Examples

List incidents:

```http
GET /api/incidents
```

Filter by priority:

```http
GET /api/incidents?priority=High
```

Search:

```http
GET /api/incidents?search=login
```

Pagination:

```http
GET /api/incidents?sortBy=createdAt&sortDirection=desc
```

Combined query:

```http
GET /api/incidents?status=Open&priority=High&page=1&pageSize=10&sortBy=createdAt&sortDirection=desc
```

Comments:

```http
POST /api/incidents/{incidentId}/comments
GET /api/incidents/{incident}/comments
DELETE /api/comments/{commentId}
```

## Observability and Error Handling

IncidentHub includes production-style diagnostics:

- Global exception handling
- Standard JSON error responses
- Correlation IDs using `X-Correlation-ID`
- Structured request logging with Serilog
- Slow request warnings ver 500ms
- Clean validation error responses

Example:

```json
{
  "error": "InternalServerError",
  "message": "An unexpected error occurred.",
  "correlationId": "6c0c70ce-5e5e-4e6b-a8cc-1c90eec6d9af",
  "timestampUtc": "2026-05-20T18:12:00Z"
}
```

## Test Dataset

To reproduce the performance issue, I created a development-only large dataset seeder.

The seeder generated:

- 100 users
- 5,000 incidents
- 20,000 comments

Incident statuses were weighted to approximate a real support queue:

- Open: 40%
- In Progress: 25%
- Resolved: 25%
- Closed: 10%

Priorities were also weighted:

- Low: 35%
- Medium: 35%
- High: 25%
- Critical: 8%

This made the incident list endpoint realistic enough to expose inefficient query behavior.
