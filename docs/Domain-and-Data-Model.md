# Domain and data model

[Home](Home.md) · [Architecture](Architecture.md) · [Persistence and migrations](Persistence-and-Migrations.md)

AlumniService groups member profiles and their employment, exam and further-study records in `Alumni.Network`. Staff profiles live in the `Alumni.Staff` namespace within the `Alumni.Staff` project. The domain libraries contain request records, validators, handlers, response mappers and EF configuration for their own features.

## Relationships at a glance

This conceptual diagram shows the domain relationships. Addresses share the member table; the address boxes do **not** represent separate database tables.

```mermaid
erDiagram
    MEMBER ||--o{ COMPANY : has
    MEMBER ||--o{ EXAM : has
    MEMBER ||--o{ FURTHER_STUDY_ENTITY : has
    MEMBER ||--|| CURRENT_ADDRESS : owns
    MEMBER ||--|| CORRESPONDENCE_ADDRESS : owns
    MEMBER {
        int Id PK
        uuid MemberId UK
        string Email UK
        string Branch
        date DateOfBirth
    }
    COMPANY {
        int Id PK
        uuid CompanyId UK
        int MemberId FK
        string CompanyName
        long AnnualSalary
    }
    EXAM {
        int Id PK
        uuid ExamId UK
        int MemberId FK
        string ExamName
        int Score
    }
    FURTHER_STUDY_ENTITY {
        int Id PK
        uuid FurtherStudyId UK
        int MemberId FK
        string InstituteName
        string Degree
    }
    FACULTY {
        int Id PK
        uuid FacultyId UK
        string Email UK
        long MobileNo
    }
```

Each related-record handler first resolves the member's public GUID to the internal integer key. A missing member returns `NotFound`. Company creation sets the foreign key and navigation explicitly; exam and further-study creation add the new entity to the tracked member's collection. EF relationship tracking supplies their foreign keys when saving.

The committed [model snapshot](../Source/Apps/Alumni.Api/Migrations/ApplicationContextModelSnapshot.cs) defines required member foreign keys with cascade delete. There is currently no member-delete handler. Faculty has no mapped relationship to members or Identity users.

## Records, entities and identifiers

| Feature | Source model | Persisted type | Public identifier |
|---|---|---|---|
| Member | `Member` | `MemberEntity` | `MemberId` |
| Company | `Company` | `CompanyEntity` | `CompanyId` |
| Exam | `Exam` | `ExamEntity` | `ExamId` |
| Further study | `FurtherStudy` | `FurtherStudyEntity` | `FurtherStudyId` |
| Staff | `StaffMember` | `StaffMemberEntity` | `FacultyId` |

Database `Id` values are generated integers. Public identifiers are GUIDs with unique indexes; clients use these identifiers for lookups. Member, company, exam and further-study creation generates version 7 GUIDs. Faculty creation uses `Guid.NewGuid()`. Creation requests generated from member-related models can expose public identifier properties, but handlers generate the persisted identifiers themselves.

`[RecordView]` copies selected model properties into partial request, response or entity records at compilation. Handwritten mappers still decide values and normalization. See [Source generation](Source-Generation.md).

Further study has an important persistence detail: configuration targets the base `FurtherStudy` record, while the context exposes `FurtherStudyEntity`. EF maps this inheritance to one `Network.further_studies` table with a `Discriminator` column. The derived type adds the member relationship; this is not a second table. See [FurtherStudy configuration](../Source/Libraries/Alumni.Network/FurtherStudies/FurtherStudyConfiguration.cs) and the snapshot.

## Validation and normalization

Callers invoke handlers through `Execute`, which validates before persistence. The shared rules are in [ValidationExtensions](../Source/Libraries/Core/ValidationExtensions.cs); each feature adds its own constraints.

New records trim surrounding whitespace from text. Names, addresses, branch and other free text retain casing, Unicode, punctuation and internal spaces. These creation policies do not rewrite existing rows.

| Input | Current policy |
|---|---|
| Member/faculty first and last names | Required; at most 100 trimmed characters each |
| Member/faculty email | Required; at most 100 trimmed characters for these records; normalized and syntax-validated by `Email` |
| Extension | Required text; at most 10 trimmed characters |
| Member mobile | 1–15 ASCII digits after trimming; cannot be all zeros; stored as a string |
| Faculty mobile | Positive `long`; the member string rule does not apply |
| Member branch | Required; at most 30 trimmed characters; no fixed allowed list |
| Member gender | Case-insensitive `Male` or `Female`; persists canonical casing |
| Member birth date | Age greater than 10 and no older than 100 relative to the current UTC date |
| Years | 1900 through the current UTC year |
| Member chronology | Admission cannot precede birth year; passing cannot precede admission |
| Member addresses | Both required; country, state, city and address line have 100-character trimmed limits; postal code is required free text |
| Company | Name ≤50, designation ≤30 trimmed characters; annual salary ≥0; valid joining year |
| Exam | Required name ≤100 trimmed characters; score 0–32767; valid year |
| Further study | Institute/degree ≤50, country/city ≤30 trimmed characters; passing year cannot precede admission |
| Lookup/related-record identifiers | Nonempty GUIDs |

### Email policy

The [Core `Email` value](../Source/Libraries/Core/Email.cs) trims whitespace and lowercases the whole address using invariant casing. Equality, conversions and display use that normalized value. Dots and plus aliases are preserved.

Syntax permits unquoted ASCII local parts with nonempty dot-separated segments and a dotted DNS domain. Domain labels permit letters, digits and internal hyphens, with at most 63 characters per label. The value allows up to 64 characters before `@` and 254 overall; member/faculty requests impose the smaller 100-character limit to match their columns. Punycode domains are accepted. Quoted local parts, raw Unicode addresses and IP literals are outside this policy. Syntax validation does not prove ownership or deliverability.

Member and faculty handlers check for an existing normalized email before insertion; each table also has a unique email index. The application does not require email uniqueness across both tables.

## Audit and deletion behavior

Member and faculty implement `IAuditableEntity` with `CreatedAt`, `UpdatedAt` and `IsDeleted`. Creation handlers set both timestamps to UTC. The model contains deletion flags, but the context has no global soft-delete filter or automatic audit interceptor. Faculty deletion calls `Remove` and physically deletes the row, returning the deleted record. Company, exam and further-study records do not carry these audit fields.

## Pagination

Member and faculty lists default to page 1 and size 10 at the domain input level. Validators require page ≥1, size 1–100, and an offset that fits in an integer. Those handlers order by internal `Id` before pagination.

Company and exam lists use fixed page 1, size 10; further-study lists use page 1, size 50. Their queries currently have no explicit sort order. These handlers first check that the member exists, so an existing member without related rows produces an empty page.

[PaginationQuery](../Source/Libraries/Core/PaginationQuery.cs) counts matching rows, then runs `Skip`/`Take`. [PaginatedList](../Source/Libraries/Core/PaginatedList.cs) exposes items, total count, page number, page size, total pages and previous/next-page flags. Mapping preserves this metadata. These are offset pages, with separate count and item queries.

## Source guide

- [Member model](../Source/Libraries/Alumni.Network/Member.cs), [entity](../Source/Libraries/Alumni.Network/MemberEntity.cs), [creation handler and mapper](../Source/Libraries/Alumni.Network/AddMemberHandler.cs)
- [Company slice](../Source/Libraries/Alumni.Network/Companies/AddCompanyHandler.cs), [exam slice](../Source/Libraries/Alumni.Network/Exams/AddExamHandler.cs), [further-study slice](../Source/Libraries/Alumni.Network/FurtherStudies/AddFurtherStudyHandler.cs)
- [Staff model](../Source/Libraries/Alumni.Staff/StaffMember.cs), [persisted entity](../Source/Libraries/Alumni.Staff/StaffMemberEntity.cs), [creation](../Source/Libraries/Alumni.Staff/AddStaffMemberHandler.cs), [deletion](../Source/Libraries/Alumni.Staff/DeleteStaffMemberHandler.cs)

---

Next: [Persistence and migrations](Persistence-and-Migrations.md) · [Home](Home.md)
