# Invitations and account activation

[Home](Home.md) · [Architecture](Architecture.md) · [Persistence and migrations](Persistence-and-Migrations.md)

Student and faculty accounts are provisioned by invitation. A profile and a pending auth account are created in one PostgreSQL transaction. The profile receives the account's `AuthUserId` scalar; no foreign key crosses between the domain and auth schemas. The account has no password and stays `PendingActivation` until the invitation is accepted. The first administrator uses the separate bootstrap API below.

## Provisioning

`Infrastructure.Auth.IInvitationProvisioningService` provides `InviteStudentAsync` and `InviteFacultyAsync`. Student/faculty profile inputs use the existing domain validation. The service rejects an email already used by any account or either kind of profile. Invitations require the initiating account to be an active faculty administrator; the role is checked in the database. Faculty invitations accept only `FacultyReader`, `FacultyEditor` or `FacultyAdmin`.

Issuance returns an `InvitationIssued` result to the trusted internal caller, including its one-time token for out-of-band delivery. No invitation-creation endpoint, email sender, GraphQL mutation or gRPC method is exposed yet. Admin HTTP endpoints will follow authentication setup. The existing domain creation endpoints are unchanged and still bypass account provisioning until authorization enforcement is implemented.

Tokens contain 32 cryptographically random bytes encoded as base64url. Only their SHA-256 digest is persisted. Invitations expire after 48 hours. Do not put tokens or passwords into application logs; request/result `ToString()` implementations omit these secrets.

## Bootstrap the first administrator

The auth storage migration must be generated, reviewed and applied before using this workflow. This step adds no model changes or migrations; see [Persistence and migrations](Persistence-and-Migrations.md). Configure the API's PostgreSQL connection as for direct API startup.

Send an anonymous HTTPS `POST /auth/bootstrap` with a JSON body:

```json
{
  "email": "admin@example.com",
  "password": "<new-password>"
}
```

`BootstrapAdminHandler` validates and normalizes the request, then the persistence store atomically checks for an existing `FacultyAdmin`, hashes the password using Identity and creates the first active admin. Any existing admin role assignment blocks bootstrap, including pending or disabled accounts. The guard and creation use the same transaction-level advisory lock as invitation writes, so concurrent bootstrap requests cannot both succeed.

Success returns HTTP 201 with `{"userId":"<account-id>","email":"admin@example.com"}`. An existing admin returns HTTP 409. Invalid input returns HTTP 400. The endpoint allows five attempts per minute per directly observed remote IP and returns 429 when exceeded.

Bootstrap accepts only email and password. It creates an auth account without a faculty profile or invitation; profile details can be supplied later without inventing required domain data. Email remains unconfirmed because this public request does not establish email ownership. It returns no access or refresh tokens. There is no command-line bootstrap or bootstrap-recovery bypass.

## Accept an invitation

Send an HTTPS JSON POST to `/auth/invitations/accept`:

```json
{
  "token": "<issued-token>",
  "password": "<new-password>"
}
```

The invitation token is the credential for this anonymous endpoint. Passwords and tokens belong in the JSON body, not query parameters. The endpoint allows ten attempts per minute per directly observed remote IP, with no request queue; excess attempts return 429. Behind a reverse proxy, forwarded-header trust must be configured before relying on per-client IP partitioning.

Acceptance verifies expiry, revocation, prior acceptance, account state, email binding and the assigned role. Passwords use ASP.NET Core Identity validation and hashing: minimum 12 characters, with the default uppercase, lowercase, digit and non-alphanumeric requirements, and a maximum input length of 256 characters. A successful transaction sets `PasswordHash`, confirms the account email, changes status to `Active` and records `AcceptedAt`.

Success returns HTTP 200 with `{"userId":"<account-id>"}`. Unavailable links return the same generic HTTP 400 response whether missing, expired, revoked or already consumed. Invalid passwords return HTTP 400 with validation feedback. Activation does not issue access or refresh tokens; login and token issuance are the next step.

## Resend, revoke and consistency

`Alumni.Auth.Invitations.IInvitationService` also provides internal `ResendAsync` and `RevokeAsync` operations. Both require an active faculty administrator. Resending keeps the account/profile and invalidates outstanding links before issuing a fresh 48-hour invitation. Revoking invalidates the account's outstanding links and leaves the pending account and profile intact. Active accounts cannot be reactivated through invitation acceptance.

All invitation writes acquire the same transaction-level PostgreSQL advisory lock, serializing bootstrap, acceptance, resend and revocation. Identity concurrency stamps and invitation `xmin` concurrency provide additional checks. Password and account/profile changes roll back together on failure. These operations own their scoped unit of work; do not call them while unrelated changes are pending in the same DbContext. Failed operations clear tracked state before the scope can be reused. Future account-role/status administration must coordinate with the same lock.

Database-free tests cover bootstrap validation, endpoint binding, error sanitization, cancellation, tokens and service registration. PostgreSQL integration checks are still needed for transaction rollback, concurrent acceptance/resend/revoke, duplicate provisioning and concurrent first-admin bootstrap.
