# IncidentHub API

A production-style ASP.NET Core backend project focused on backend debugging, API reliability, authentication issues, database performance, Docker deployment, and observability.

## Authentication

IncidentHub uses JWT authentication with role-based authorization.

Supported auth endpoints:

```
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/me
```

Roles:

- User: can create and view incidents
- Admin: can delete incidents and perform administrative actions

Security notes:

- Passowrds are hashed using BCrypt
- JWTs include user ID, email, and role claims
- Incident ownership is derived from the authenticated user token
