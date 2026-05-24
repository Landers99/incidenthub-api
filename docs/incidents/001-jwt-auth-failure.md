# Incident 001: JWT Auth Failure

## Summary

Users could log in successfully and receive a JWT, but protected API endpoints returned 401 Unauthorized when the token was used.

## Impact

Authenticated users could not access protected incident endpoints.

## Symptoms

- Login endpoint returned 200 OK.
- JWT access token was generated.
- Protected endpoint returned 401 Unauthorized.
- Controller action was not reached.

## Investigation

1. Confirmed the Authorization header was present and used the Bearer scheme.
2. Confirmed the endpoint requried authentication and correctly returned 401 when token validation failed.
3. Decoded token contained expected user claims and had not expired.
4. The token issuer did not match the issuer configuerd in taken validation.
5. Found that the generated token issuer did not match `ValidIssuer`.

## Root Cause

The token was generated with issuer `IncidentHub`, but the authentication middleware expected issuer `IncidentHub.Api`.

## Fix

Updated token generation and validation to use the same centralized JWT configuration.

## Verification

- Login returned a valid token
- Protected endpoints returned 200 OK with the token.
- Missing token returned 401.
- Invalid token returned 401.
- Non-admin user accessing admin route returned 403.

## Prevention

- Added startup validation for JWT settings.
- Added logging for authentication failures.
- Removed duplicated hardcoded issuer strings.
