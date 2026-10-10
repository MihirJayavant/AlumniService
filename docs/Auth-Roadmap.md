# Authentication and authorization: remaining work

[Home](Home.md) · [Invitations](Invitations.md) · [Architecture](Architecture.md) · [API transports](API-Transports.md)

Handoff recorded on **2026-10-10**. Auth implementation is deferred while other work proceeds. This page records the agreed direction and the remaining tasks for a future session. Check the current source and Git changes before resuming; do not assume this page describes later edits.

## Current implementation

- `Alumni.Api` hosts REST, GraphQL and gRPC. All three call shared domain handlers.
- `Alumni.Auth` contains Identity account/role entities, invitation/session entities, OpenIddict model mappings, shared permission policies and bootstrap/invitation contracts.
- Auth tables are mapped to PostgreSQL's quoted `Auth` schema. Student/faculty profiles hold a nullable, unique scalar `AuthUserId`; there are no foreign keys between auth and domain profiles.
- Accounts have `PendingActivation`, `Active` and `Disabled` states. Invited accounts initially have no password.
- `ICurrentActor` and the API's `CurrentActor` expose authentication state and the account ID from the authenticated `sub` claim. They do not validate tokens.
- Internal invitation services create account/profile/invitation together, accept invitations, resend and revoke. Tokens contain 256 random bits; only SHA-256 hashes are stored; expiry is 48 hours.
- Anonymous `POST /auth/invitations/accept` sets an Identity password and activates the account. Invitation creation/resend/revocation have no public admin endpoints yet.
- Anonymous `POST /auth/bootstrap` accepts **email and password only**, through `BootstrapAdminHandler`. It creates the first active `FacultyAdmin` auth account, without a profile, invitation or tokens. Any existing admin role assignment blocks bootstrap, including pending/disabled accounts. There is no CLI bootstrap or bootstrap-recovery bypass.
- Bootstrap and invitation writes share PostgreSQL advisory transaction lock `71422024001`. Future account role/status writes must coordinate with it. These services own their scoped unit of work and clear tracked state when transactions finish.

**JWT issuance/validation and domain authorization enforcement are not implemented. Current domain endpoints remain unprotected, and legacy profile creation paths bypass invitation provisioning.**

## Decisions to preserve

Keep authentication hosted in `Alumni.Api`, with reusable auth code in `Alumni.Auth` and EF/persistence orchestration in Infrastructure. A separate deployed auth service is unnecessary at this stage.

Use ASP.NET Core Identity for password management, OpenIddict for JWT access tokens and refresh tokens, and shared ASP.NET Core authorization policies. The built-in Identity API bearer tokens do not satisfy the JWT requirement.

Use one account/token system across REST, GraphQL and gRPC. Separate student and faculty login UIs must not determine permissions; the server uses the account's assigned roles. Invitation-only registration applies to students and faculty, with the first-admin bootstrap exception above.

| Role | Intended access |
|---|---|
| `Student` | Read own profile/related records; edit approved own fields only |
| `FacultyReader` | Read all student data |
| `FacultyEditor` | Read/write student data |
| `FacultyAdmin` | Read/write student data, invite accounts, manage faculty roles/status |

Student self-editing covers approved contact/address, employment and further-study fields. Define the exact command fields before implementation. Academic/identity fields and exam edits must not become editable implicitly. In particular, distinguish profile contact-email changes from login-email changes.

Keep account/profile identifiers independent. Resolve ownership through `Student.AuthUserId == authenticated sub`; never treat a client-supplied profile/account ID as proof of ownership.

## 1. Establish the database and verification baseline

- [ ] Review current changes, including any user adjustments after the last successful test run. Preserve unrelated work.
- [ ] Reconcile current bootstrap response behavior and its tests/docs: endpoint metadata advertises 201, but the current `ToServerResult()` success path returns 200. Choose and implement one consistent contract.
- [ ] Recheck password-safe formatting and error sanitization after the recent controller/request simplifications. Do not log request passwords or invitation secrets.
- [ ] Generate/review an auth-storage migration. Only `InitialCreate` and its snapshot were present at handoff; auth model changes have not been captured in a new migration.
- [ ] Apply the reviewed migration to the intended local PostgreSQL database, following repository instructions and explicit database authorization.
- [ ] Verify atomic account/profile/invitation creation and rollback when any part fails.
- [ ] Verify invitation expiry, invalid passwords, single-use acceptance, resend invalidation and revocation.
- [ ] Verify concurrent bootstrap requests create exactly one admin; existing pending/active/disabled admins all block bootstrap.
- [ ] Verify concurrent acceptance/resend/revocation and retry after failed operations.

Read [Persistence and migrations](Persistence-and-Migrations.md) and the `alumni-migration` skill before migration work. Generating a migration does not authorize applying it.

## 2. Implement login and token management

- [ ] Register OpenIddict server and validation runtime integration alongside the existing EF storage. Pin new packages centrally in `Directory.Packages.props`.
- [ ] Implement email/password verification, Identity lockout and throttling, rejecting pending/disabled accounts.
- [ ] Complete the client login protocol. Authorization code with PKCE was proposed for browser clients; confirm client hosting and token storage before implementing the UI flow.
- [ ] Issue JWT access tokens with account ID in `sub`, explicit audience and role claims. Configure consistent claim types across issuance and validation.
- [ ] Implement refresh-token rotation, reuse detection and session/grant revocation.
- [ ] Implement logout and session revocation; decide whether to expose device/session listing and logout-all now or later.
- [ ] Enforce account/session validity on requests so disablement, logout and role changes invalidate existing access. Revoke sessions on role changes to eliminate stale privileges.
- [ ] Configure issuer, audience, signing/encryption credentials, key rotation and token lifetimes. Suggested starting values were 10-minute access tokens and a 7-day absolute session expiry; these are proposals, not implemented settings.
- [ ] Decide the email-confirmation path for the bootstrapped admin. Bootstrap currently leaves `EmailConfirmed=false`; avoid configuring login in a way that permanently locks out this account.

Use HTTP auth/token endpoints for clients of all three transports. Login and refresh need not be duplicated as GraphQL mutations or gRPC methods.

## 3. Enforce authorization across all transports

- [ ] Set up one bearer authentication scheme and middleware for REST, GraphQL and gRPC.
- [ ] Attach operation policies to REST endpoint groups/routes.
- [ ] Add HotChocolate authorization integration and policies on GraphQL queries/mutations. Preserve per-resolver DI scopes and access to the same authenticated actor.
- [ ] Apply ASP.NET Core authorization to gRPC services/methods; clients send bearer tokens through `authorization` metadata.
- [ ] Enforce resource ownership and business permissions inside shared use cases, beyond transport checks.
- [ ] Restrict student lists to permitted faculty; students access only their own records.
- [ ] Check the owning student for employment, exams and further-study operations, including writes using submitted IDs.
- [ ] Add dedicated student self-edit commands containing only approved fields; keep general faculty editing separate.
- [ ] Return consistent REST 401/403, GraphQL authorization errors and gRPC `Unauthenticated`/`PermissionDenied` outcomes without leaking other students' data.

Existing policies are eligibility checks: `Students.Read`, `Students.ReadAll`, `Students.Write`, `Students.SelfService`, `Faculty.Manage` and `Invitations.Manage`. Merely registering them does not enforce ownership or protect an endpoint.

## 4. Expose authenticated administration

- [ ] Add admin endpoints for student/faculty invitation creation, resend and revocation, using the existing internal services.
- [ ] Add faculty role/status management, including session revocation and coordinated database locking.
- [ ] Protect the last active faculty admin from being disabled, deleted or demoted.
- [ ] Replace/restrict unrestricted profile creation paths so account provisioning cannot be bypassed.
- [ ] Decide how existing unlinked profiles are onboarded; do not create duplicate profiles or assume all existing `AuthUserId` values are populated.
- [ ] Provide a way to complete/link the bootstrapped admin's faculty profile using actual profile data.

## 5. Complete recovery and delivery

- [ ] Implement invitation email delivery and activation links, keeping raw secrets out of logs.
- [ ] Add forgot/reset/change-password flows and invalidate affected sessions as appropriate.
- [ ] Define login-email change/verification behavior separately from profile editing.
- [ ] Implement the separate student/faculty login UIs against the shared account system when frontend work is in scope.

## 6. Verify and prepare deployment

- [ ] Test all four roles across all three transports, including anonymous users and cross-student access.
- [ ] Test malformed/expired/revoked tokens, pending/disabled accounts, refresh reuse and role changes.
- [ ] Test student field restrictions and related-record ownership.
- [ ] Test actual rate-limit middleware behavior and configure trusted forwarded headers if deployed behind a proxy.
- [ ] Configure HTTPS, CORS for intended clients, browser token/refresh handling and any required CSRF protections for cookie-backed flows.
- [ ] Store production credentials/keys outside the repository and update API/client documentation for the final contracts.

## Verification history and commands

The last agent-verified implementation built with zero warnings/errors and passed **563 unit tests** across five projects. That result preceded later user edits to bootstrap/invitation controllers and the bootstrap request. It is historical evidence; rerun checks on the current tree. PostgreSQL integration checks were not executed, and no migration/database changes were applied.

Run repository checks from the repository root:

```sh
dotnet build.cs -- --target=Test
```

In the sandbox, the Cake build wrapper stalled and `dotnet test` could not bind its IPC socket. The following supported direct build/test execution worked within the sandbox:

```sh
dotnet build AlumniService.slnx --no-restore --disable-build-servers -m:1
DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false dotnet Tests/Alumni.Api.UnitTests/bin/Debug/net10.0/Alumni.Api.UnitTests.dll
dotnet Tests/Alumni.Student.UnitTests/bin/Debug/net10.0/Alumni.Student.UnitTests.dll
dotnet Tests/Alumni.Faculty.UnitTests/bin/Debug/net10.0/Alumni.Faculty.UnitTests.dll
dotnet Tests/Core.UnitTests/bin/Debug/net10.0/Core.UnitTests.dll
dotnet Tests/Generators.UnitTests/bin/Debug/net10.0/Generators.UnitTests.dll
```

Disabling configuration reload prevented test-host file-watcher stalls. Keep builds, tests and EF commands serialized. Follow the repository sandbox/manual-action instructions if a required command remains blocked.

## Resume in a new session

Start with this page, [Invitations](Invitations.md), root/scoped `AGENTS.md`, and the relevant repository skills. Inspect Git changes and the current source, establish the baseline in step 1, then implement login and tokens in step 2. Continue in small, reviewable steps; do not assume authorization is already enforced.

Useful source starting points:

- [Auth entities, policies and contracts](../Source/Libraries/Alumni.Auth/)
- [Bootstrap handler](../Source/Libraries/Alumni.Auth/Bootstrap/BootstrapAdmin.cs) and [persistence](../Source/Libraries/Infrastructure/Auth/AdminBootstrapStore.cs)
- [Invitation persistence/orchestration](../Source/Libraries/Infrastructure/Auth/InvitationService.cs)
- [Current actor](../Source/Apps/Alumni.Api/Services/CurrentActor.cs) and [shared interface](../Source/Libraries/Core/ICurrentActor.cs)
- [API wiring](../Source/Apps/Alumni.Api/Program.cs) and [Infrastructure registration](../Source/Libraries/Infrastructure/ConfigureServices.cs)
- [Auth unit tests](../Tests/Alumni.Api.UnitTests/Auth/)
