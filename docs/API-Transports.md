# API Transports

[Home](Home.md) · [Architecture](Architecture.md) · [README](../README.md)

HTTP minimal APIs, GraphQL and gRPC are all registered in the current [`Program.cs`](../Source/Apps/Alumni.Api/Program.cs). They reuse the same domain handlers rather than implementing separate business rules.

## Operation map

| Feature | HTTP routes | GraphQL fields | gRPC service methods |
|---|---|---|---|
| Student | `GET /student/`, `GET /student/{id}`, `POST /student/` | `students`, `student`, `addStudent` | `StudentService.List`, `Get`, `Add` |
| Faculty | `GET /faculty/`, `GET /faculty/{facultyId}`, `POST /faculty/`, `DELETE /faculty/{facultyId}` | `faculties`, `faculty`, `addFaculty`, `deleteFaculty` | `FacultyService.List`, `Get`, `Add`, `Delete` |
| Company | `GET /company/{studentId}`, `POST /company/` | `companies`, `addCompany` | `CompanyService.ListByStudent`, `Add` |
| Exam | `GET /exam/{studentId}`, `POST /exam/` | `exams`, `addExam` | `ExamService.ListByStudent`, `Add` |
| Further study | `GET /further-studies/{studentId}`, `POST /further-studies/` | `furtherStudies`, `addFurtherStudy` | `FurtherStudyService.ListByStudent`, `Add` |

HTTP route identifiers use GUID constraints. Related-record GET routes list records belonging to a student. Student and faculty HTTP lists take `pageNumber` and `pageSize` query parameters. Successful HTTP handler results return 200, including adds and faculty deletion.

```sh
curl 'https://localhost:<api-port>/student/?pageNumber=1&pageSize=10'
```

Use the API HTTPS URL reported by Aspire or the direct launch profile. In development, OpenAPI is available at `/openapi/v1.json`, with Swagger UI at `/swagger` and Scalar at `/scalar`.

## Authentication and authorization

Identity and OpenIddict storage, shared permission policies and a transport-independent current actor are defined. JWT issuance and validation are not enabled yet. Current domain routes, GraphQL fields and gRPC services do not attach authorization requirements. Registering policies does not itself protect an endpoint. See [Authorization foundation](Architecture.md#authorization-foundation) for the role mapping and deferred ownership checks.

## Error conversion

| Handler status | HTTP | GraphQL `extensions.code` | gRPC |
|---|---|---|---|
| Bad request | 400 | `BAD_REQUEST` | `InvalidArgument` |
| Not found | 404 | `NOT_FOUND` | `NotFound` |
| Conflict | 409 | `CONFLICT` | `AlreadyExists` |
| Unauthorized | 401 | `UNAUTHORIZED` | `Unauthenticated` |
| Forbidden | 403 | `FORBIDDEN` | `PermissionDenied` |
| Internal error | 500 | `INTERNAL_ERROR` | `Internal` |

HTTP internal errors currently include the handler message in problem details. GraphQL and gRPC use a generic internal-error message. Input binding, GraphQL parsing/scalar checks and gRPC input conversion can reject a request before a domain handler runs.

## gRPC API

The API exposes all 13 controller operations through five unary gRPC services in package `alumni.v1`: `StudentService` (`List`, `Get`, `Add`), `FacultyService` (`List`, `Get`, `Add`, `Delete`), and `CompanyService`, `ExamService`, and `FurtherStudyService` (each with `ListByStudent` and `Add`). Contracts use snake_case filenames under `Source/Apps/Alumni.Api/Grpc/Protos/alumni/v1`, matching their package. Clients can generate stubs with `Grpc/Protos` as the import root. The API project includes proto files recursively; `common.proto` generates messages only. `AddApplicationGrpc` registers gRPC infrastructure and `MapApplicationGrpc` maps the five services.

Each service calls the same domain handlers through `Execute`, including validation and persistence, and passes the call cancellation token. Use an HTTPS endpoint supporting HTTP/2. Access requirements match the current controller endpoints.

GUIDs use strings; student birth dates use `yyyy-MM-dd`; faculty audit dates use protobuf timestamps, with `updated_at` absent when null. Faculty mobile numbers and annual salaries use `int64`; student mobile numbers remain strings. Client-supplied creation IDs are accepted but the handlers generate the persisted IDs.

Student and faculty `List` requests default omitted page fields to page 1 and size 10; explicitly supplied values are validated by the handlers. Related-record lists preserve the current fixed pagination: page 1, size 10 for companies/exams and size 50 for further studies. Every list reply includes items and pagination metadata. Faculty `Delete` returns the deleted faculty record.

Invalid input produces `InvalidArgument`, missing records produce `NotFound`, duplicate records produce `AlreadyExists`, and authentication/authorization failures map to `Unauthenticated`/`PermissionDenied`. Internal handler failures produce `Internal` with a generic message. Cancellation is allowed to propagate to gRPC.

## GraphQL API

The API exposes all 13 controller operations through HotChocolate at `/graphql`, alongside HTTP and gRPC. Resolvers call the same domain handlers through `Execute`, preserving validation, mapping, persistence, and cancellation. Authentication and authorization requirements match the current controller endpoints.

Queries are `students(pageNumber, pageSize)`, `student(id)`, `faculties(pageNumber, pageSize)`, `faculty(facultyId)`, `companies(studentId)`, `exams(studentId)`, and `furtherStudies(studentId)`. Mutations are `addStudent(input)`, `addFaculty(input)`, `deleteFaculty(facultyId)`, `addCompany(input)`, `addExam(input)`, and `addFurtherStudy(input)`. Faculty deletion returns the deleted record.

Student and faculty list pagination arguments are required. Lists return `items`, `totalCount`, `pageNumber`, `pageSize`, `totalPages`, `hasPreviousPage`, and `hasNextPage`. Related-record lists preserve the handlers' fixed pagination: page 1 with size 10 for companies/exams and size 50 for further studies. No extra filtering, sorting, or cursor pagination is applied.

Example query:

```graphql
query Students($pageNumber: Int!, $pageSize: Int!) {
  students(pageNumber: $pageNumber, pageSize: $pageSize) {
    items { studentId firstName lastName email dateOfBirth }
    totalCount
    pageNumber
    pageSize
    hasNextPage
  }
}
```

Variables: `{"pageNumber": 1, "pageSize": 10}`. Send an HTTP POST to `/graphql` with a JSON body containing `query` and `variables`, using the API URL from Aspire or your direct API launch.

Example mutation:

```graphql
mutation AddFaculty($input: AddFacultyInput!) {
  addFaculty(input: $input) {
    facultyId
    email
    firstName
    createdAt
  }
}
```

Example variables:

```json
{
  "input": {
    "email": "faculty@example.com",
    "firstName": "Ada",
    "lastName": "Lovelace",
    "extension": "123",
    "mobileNo": 919876543210
  }
}
```

Input fields mirror the corresponding domain request records, including creation IDs where the HTTP requests expose them; handlers still generate persisted IDs. GUIDs use the `UUID` scalar, birth dates use `LocalDate` (`yyyy-MM-dd`), timestamps use `DateTime`, and 64-bit values use `Long`. Student mobile numbers remain strings. Nullable faculty `updatedAt` values remain nullable.

Handler failures appear in GraphQL `errors` with a field `path` and an `extensions.code`: `BAD_REQUEST`, `NOT_FOUND`, `CONFLICT`, `UNAUTHORIZED`, or `FORBIDDEN`. Other failures use `INTERNAL_ERROR` with a generic message. Root fields are nullable so successful sibling fields can still return data. Malformed operations or scalar values are rejected by GraphQL before handler execution and use HotChocolate's own error codes. GraphQL errors do not mirror controller HTTP status codes.

Each resolver uses its own service scope, preventing concurrent query fields from sharing an EF DbContext and isolating tracked changes between mutation fields. Multiple mutations do not form one transaction; each handler retains its own save boundary.

## Source references

- [Explicit HTTP endpoint registration](../Source/Apps/Alumni.Api/Controllers/Endpoint.cs)
- [HTTP result conversion](../Source/Apps/Alumni.Api/Controllers/ResultHelper.cs)
- [GraphQL registration and resolver scopes](../Source/Apps/Alumni.Api/ExtensionService/GraphQLExtension.cs)
- [GraphQL errors](../Source/Apps/Alumni.Api/GraphQL/GraphQLResult.cs)
- [gRPC registration](../Source/Apps/Alumni.Api/ExtensionService/GrpcExtension.cs)
- [gRPC errors](../Source/Apps/Alumni.Api/Grpc/GrpcResult.cs) and [input conversion](../Source/Apps/Alumni.Api/Grpc/GrpcInput.cs)
- [Protocol contracts](../Source/Apps/Alumni.Api/Grpc/Protos/alumni/v1)
