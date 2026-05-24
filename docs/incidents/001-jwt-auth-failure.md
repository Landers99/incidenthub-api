# Incident 001: JWT Auth Failure

## Summary

Users could log in successfully and receive a JWT, but protected API endpoints returned 401 Unauthorized when the token was used.

## Symptoms

- Login endpoint returned 200 OK.
- JWT access token was generated.
- Protected endpoint returned 401 Unauthorized.
- Controller action was not reached.
- Failure occured during authentication middleware validation.

## Investigation

- Confirmed the Authorization header was present and used the Bearer scheme.
- Confirmed the endpoint requried authentication and correctly returned 401 when token validation failed.
- Decoded token contained expected user claims and had not expired.
- The token issuer did not match the issuer configuerd in taken validation.
