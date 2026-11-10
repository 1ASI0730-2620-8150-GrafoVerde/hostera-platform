# Architecture Decision Records (ADRs)

## About this document

This document records the significant architectural decisions of the Hostera Platform RESTful API. Each record follows the lightweight format proposed by Michael Nygard: **Context**, **Decision**, **Consequences**, plus a **Status** and **Date**.

Guidelines for maintaining this log:

- One decision per record; records are short and focused on the *why*.
- Records are immutable once **Accepted**. To change a decision, add a new ADR that **Supersedes** the old one and update the old one's status.
- Statuses: `Proposed`, `Accepted`, `Deprecated`, `Superseded by ADR-XXX`.
- Number records sequentially and never reuse numbers.

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [ADR-001](#adr-001--organize-the-code-by-bounded-context-with-a-layered-ddd-structure) | Organize the code by bounded context with a layered DDD structure | Accepted |
| [ADR-002](#adr-002--separate-command-and-query-services-cqs) | Separate command and query services (CQS) | Accepted |
| [ADR-003](#adr-003--model-expected-failures-with-a-result-type-and-an-error-enum-per-context) | Model expected failures with a `Result` type and an error enum per context | Accepted |
| [ADR-004](#adr-004--report-errors-as-problemdetails-with-a-stable-code-in-the-title) | Report errors as `ProblemDetails` with a stable code in the title | Accepted |
| [ADR-005](#adr-005--map-rest-resources-with-static-assemblers) | Map REST resources with static assemblers | Accepted |
| [ADR-006](#adr-006--persist-with-ef-core-on-mysql-using-repository-and-unit-of-work) | Persist with EF Core on MySQL using Repository and Unit of Work | Accepted |
| [ADR-007](#adr-007--use-snake_case-database-naming-and-automatic-audit-timestamps) | Use `snake_case` database naming and automatic audit timestamps | Accepted |
| [ADR-008](#adr-008--create-the-schema-on-startup-with-ensurecreated) | Create the schema on startup with `EnsureCreated` | Accepted |
| [ADR-009](#adr-009--use-plural-kebab-case-routes-scoped-by-property) | Use plural kebab-case routes scoped by property | Accepted |
| [ADR-010](#adr-010--use-built-in-openapi-and-scalar) | Use built-in OpenAPI and Scalar | Accepted |
| [ADR-011](#adr-011--integrate-bounded-contexts-through-anti-corruption-facades) | Integrate bounded contexts through anti-corruption facades | Accepted |
| [ADR-012](#adr-012--read-the-operational-overview-from-a-read-only-model) | Read the operational overview from a read-only model | Accepted |
| [ADR-013](#adr-013--evaluate-day-dependent-rules-in-the-time-zone-of-the-properties) | Evaluate day-dependent rules in the time zone of the properties | Accepted |
| [ADR-014](#adr-014--number-and-price-bookings-on-the-server) | Number and price bookings on the server | Accepted |
| [ADR-015](#adr-015--protect-bookings-with-optimistic-concurrency-and-payments-with-idempotency-keys) | Protect bookings with optimistic concurrency and payments with idempotency keys | Accepted |
| [ADR-016](#adr-016--describe-errors-in-the-language-of-the-request-with-resx-resources) | Describe errors in the language of the request with `.resx` resources | Accepted |
| [ADR-017](#adr-017--seed-demonstration-data-through-the-application-services) | Seed demonstration data through the application services | Accepted |
| [ADR-018](#adr-018--deploy-a-docker-container-on-render-with-a-managed-mysql-database) | Deploy a Docker container on Render with a managed MySQL database | Accepted |

---

## ADR-001 — Organize the code by bounded context with a layered DDD structure

- **Status:** Accepted
- **Date:** 2026-10-21

### Context
Hostera covers several areas of hotel operations (rooms and rates, bookings, inventory, access control, and an operational overview) that evolve at different speeds and are led by different team members. The backend must serve the bounded contexts the frontend already uses without entangling their rules.

### Decision
Use a single deployable project organized first by bounded context (`Rooms`, `Bookings`, `Inventory`, `AccessControl`, `Overview`, `Shared`) and then by layer:

- `Domain` — aggregates, entities, value objects, commands, queries, and repository contracts.
- `Application` — command and query service contracts, their implementations in `Internal`, the error enum of the context, and the outbound services that read other contexts.
- `Infrastructure` — EF Core configuration and repositories.
- `Interfaces` — REST controllers, resources, assemblers, and the anti-corruption facade offered to other contexts.

Dependencies point inward: `Interfaces` and `Infrastructure` depend on `Application` and `Domain`, never the reverse. Building blocks used by every context live in `Shared`.

### Consequences
- (+) Each context keeps the ubiquitous language of the frontend glossary and can be developed in its own Git Flow feature.
- (+) Domain rules can be reasoned about without ASP.NET Core or EF Core.
- (−) More folders and types than a CRUD layout.
- (−) The dependency rule is enforced by convention and review only.

---

## ADR-002 — Separate command and query services (CQS)

- **Status:** Accepted
- **Date:** 2026-10-21

### Context
Writes such as creating a booking or adjusting stock carry validation, uniqueness rules, and side effects, while reads are lookups and filtered lists. Mixing both in one service hides those differences.

### Decision
Expose a command service and a query service per aggregate, such as `IBookingCommandService` and `IBookingQueryService`. Commands and queries are immutable records, and every handler accepts a `CancellationToken`. Operations that span aggregates of one context, such as check-in and check-out, get their own command service (`IStayCommandService`).

### Consequences
- (+) Clear intent and smaller, focused classes.
- (+) Read paths can evolve independently, for example with paging or read models (ADR-012).
- (−) Slightly more boilerplate per use case.
- This is CQS at the service level, not full CQRS: both sides share one database and model.

---

## ADR-003 — Model expected failures with a `Result` type and an error enum per context

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
A duplicate room number, a booking that overlaps another stay, or a payment above the balance due are expected business outcomes, not exceptional conditions. Throwing exceptions for them obscures control flow.

### Decision
Command handlers return `Result<TValue, TError>` (`Shared.Application.Patterns`), where `TError` is the error enum of the context, such as `RoomError`, `RateError`, `BookingError`, `InventoryError`, `CredentialError`, or `AccessEventError`. Exceptions are reserved for unexpected failures and for resources that cannot be turned into commands.

### Consequences
- (+) Failure modes are explicit in the signature and map exhaustively to HTTP status codes.
- (+) Each enum value is also the stable error code that clients read (ADR-004).
- (−) Callers must handle both branches, and the `Result` type is maintained in the codebase.

---

## ADR-004 — Report errors as `ProblemDetails` with a stable code in the title

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
The frontend validates the same rules in the browser and shows its own English and Spanish messages. To show the same message when the server rejects a request, it needs a value that identifies the violated rule and never changes with the wording or the language.

### Decision
Every error response follows RFC 9457 problem details. The `*ActionResultAssembler` of each context maps the error enum to the HTTP status code and writes the enum name as the `title`, such as `DuplicateRoomNumber`, and an explanation as the `detail`. Bodies that fail model validation return the `InvalidInput` title with the field errors.

### Consequences
- (+) Clients recognize errors by a stable code and choose their own text.
- (+) The response format is standard and documented by OpenAPI.
- (−) The `title` is a code rather than a human-readable sentence, which differs from the usual use of the field.

---

## ADR-005 — Map REST resources with static assemblers

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
Aggregates must not be exposed directly through the API, and controllers should stay thin.

### Decision
Use dedicated request and response resources and static assembler classes in `Interfaces/Rest/Transform`, named `<Target>From<Source>Assembler`, such as `CreateBookingCommandFromResourceAssembler` and `BookingResourceFromEntityAssembler`. Kebab-case enum names in resources, such as `checked-in`, are converted by the `*EnumNames` helper of each context.

### Consequences
- (+) The public contract stays compatible with the frontend while the domain model evolves.
- (+) Controllers contain no mapping logic, and assemblers are pure functions.
- (−) No automatic mapping; each new resource needs an assembler.

---

## ADR-006 — Persist with EF Core on MySQL using Repository and Unit of Work

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
The platform needs relational persistence with uniqueness constraints, such as room numbers or inventory codes per property, and must save each use case atomically.

### Decision
Use EF Core 10 with the `MySql.EntityFrameworkCore` provider. Each aggregate has a repository contract in its `Domain` and an implementation based on `BaseRepository<T>`. Application services save through `IUnitOfWork`, whose implementation commits the changes in an explicit transaction, because the provider sends several statements as one batch and a batch that fails halfway would otherwise keep its first statements.

### Consequences
- (+) Domain and application code depend on abstractions, not on EF Core.
- (+) A rejected change, such as a payment whose booking changed meanwhile, leaves no partial writes.
- (−) The repository abstraction adds a layer over `DbContext`.

---

## ADR-007 — Use `snake_case` database naming and automatic audit timestamps

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
MySQL tables are easier to query by hand with lowercase plural names, and every record should tell when it was created and last updated without each service setting those fields.

### Decision
Apply `UseSnakeCaseNamingConvention()` to every table, column, key, and index, with pluralized table names. Entities that implement `IAuditableEntity` receive their creation and update timestamps from `AuditableEntityInterceptor` when changes are saved.

### Consequences
- (+) Consistent, predictable database names and audit data.
- (−) Database names differ from C# names, which matters when writing raw SQL (ADR-012).

---

## ADR-008 — Create the schema on startup with `EnsureCreated`

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
The model changed often while the bounded contexts were being built, and the team needed a database that matched the code without maintaining migration files.

### Decision
Call `Database.EnsureCreated()` on startup. It creates the database with every table and its reference data when the database does not exist, and changes nothing otherwise.

### Consequences
- (+) A new environment, including the deployed one, starts without manual steps.
- (−) An existing database is not updated when entities change; it must be dropped and created again, which loses its data.
- A later ADR should replace this decision with EF Core migrations once the model stabilizes.

---

## ADR-009 — Use plural kebab-case routes scoped by property

- **Status:** Accepted
- **Date:** 2026-11-02

### Context
The frontend already consumes plural kebab-case resources with camelCase fields and numeric identifiers, and almost every resource belongs to one hotel property.

### Decision
Expose versioned routes under `/api/v1`, generated in lowercase kebab-case by `KebabCaseRouteNamingConvention`. Collections that belong to a property are nested under it, such as `/properties/{propertyId}/room-types`, while a single resource is addressed by its own identifier, such as `/room-types/{roomTypeId}`. Operations that change several records at once are explicit sub-resources, such as `PUT /rooms/{roomId}/status`, `POST /bookings/{bookingId}/check-in`, or `POST /rfid-credentials/{credentialId}/replace`.

### Consequences
- (+) The contract keeps the vocabulary and shapes of the frontend.
- (+) Multi-request writes of the former mock API become single transactional operations.
- (−) The frontend had to replace flat query-string filters with nested routes.

---

## ADR-010 — Use built-in OpenAPI and Scalar

- **Status:** Accepted
- **Date:** 2026-10-28

### Context
The team and the frontend need an up-to-date description of every endpoint, resource, and error.

### Decision
Generate the OpenAPI document with `Microsoft.AspNetCore.OpenApi` and browse it with Scalar. Controllers document each operation with `EndpointSummary`, `EndpointDescription`, and `ProducesResponseType`. Both are published in Development, and elsewhere when `Hostera:PublishApiReference` is `true`.

### Consequences
- (+) The reference is generated from the code and never drifts from it.
- (+) No third-party OpenAPI generator is needed.
- (−) Publishing the reference in a deployed environment exposes the full contract, which is acceptable for a demonstration but should be disabled for production customers.

---

## ADR-011 — Integrate bounded contexts through anti-corruption facades

- **Status:** Accepted
- **Date:** 2026-11-04

### Context
Bookings must quote stays with the rates of Rooms, check room availability, issue key cards in Access Control, and mark rooms for cleaning. Rooms must know which nights a booking holds. Reaching into another context's repositories would couple their models.

### Decision
Each context that serves others publishes a facade in `Interfaces/ACL`, such as `IRoomsContextFacade`, `IBookingsContextFacade`, and `IAccessControlContextFacade`, which accepts and returns primitives or small summary records. Consumers wrap the facade in an outbound service in `Application/Internal/OutboundServices/ACL`, such as `ExternalRoomService`, and translate the summaries into their own model.

### Consequences
- (+) Contexts change their internal models without breaking each other.
- (+) The dependencies between contexts are explicit and few.
- (−) Calls between contexts share the same database transaction only when they run inside the same unit of work.

---

## ADR-012 — Read the operational overview from a read-only model

- **Status:** Accepted
- **Date:** 2026-11-04

### Context
The Overview context only reads: it combines rooms, bookings, payments, status periods, inventory, and access events into a dashboard and a comparison of properties. Loading every aggregate to compute those numbers would be slow and would couple Overview to the other domain models.

### Decision
Overview owns no aggregates. `OverviewReadRepository` reads rooms, bookings, payments, and status periods with focused SQL queries over the shared database, and the inventory, access, and property summaries come through outbound services. A section whose source cannot be read is listed in `unavailableSections` instead of failing the whole dashboard.

### Consequences
- (+) Each dashboard is computed in a few queries.
- (+) A failing source degrades one section, not the response.
- (−) The SQL queries depend on the table and column names of other contexts (ADR-007) and must follow their changes.

---

## ADR-013 — Evaluate day-dependent rules in the time zone of the properties

- **Status:** Accepted
- **Date:** 2026-11-09

### Context
Rules such as when a booking can be checked in, until when a guest key card opens a door, or which access attempts happened today depend on the local day of the hotel, not on the UTC day of the server.

### Decision
Inject `IClock`, implemented by `PropertyClock`, which reads the time zone from `Hostera:TimeZone` (`America/Lima` by default) and exposes the current local day, the current moment, and the moment of a local time on a given day.

### Consequences
- (+) Day boundaries match what the front desk sees, wherever the server runs.
- (+) Time can be replaced in tests through `TimeProvider`.
- (−) All properties share one time zone until it is stored per property.

---

## ADR-014 — Number and price bookings on the server

- **Status:** Accepted
- **Date:** 2026-11-09

### Context
The former mock API left the booking code and the total to the browser, which read the latest code and summed the nightly rates before saving. Two front desks creating bookings at the same time could produce the same code or a total based on outdated rates.

### Decision
The server assigns the next code of the property, such as `BKG-1041`, and quotes the total with the daily rates of Rooms when a booking is created or its stay, room type, or rate plan changes. When two requests take the same code, the later one retries with the next free number.

### Consequences
- (+) Codes are unique and totals reflect the rates at the time of saving.
- (−) The browser can only show an estimate before saving.

---

## ADR-015 — Protect bookings with optimistic concurrency and payments with idempotency keys

- **Status:** Accepted
- **Date:** 2026-11-09

### Context
Several operators may edit, confirm, pay, or check in the same booking, and a payment request may be repeated after a network error. Neither case may silently overwrite a newer state or record a payment twice.

### Decision
`Booking.Version` is a concurrency token returned in the `version` field and the `ETag` header. Changes that send an `If-Match` header with an outdated version are rejected with `409 Conflict` and the `ConcurrencyConflict` code. Payments accept an `Idempotency-Key` header: repeating a request with the same key returns the original payment with the `Idempotent-Replayed` header. CORS exposes these headers to browsers.

### Consequences
- (+) Lost updates and duplicated payments are prevented without locks.
- (−) Clients must keep the version and reload the booking after a conflict.

---

## ADR-016 — Describe errors in the language of the request with `.resx` resources

- **Status:** Accepted
- **Date:** 2026-11-10

### Context
The frontend is available in English and Spanish, and other clients, such as the Scalar reference, read the `detail` of errors directly.

### Decision
Register ASP.NET Core localization and select the UI culture from the `Accept-Language` header among `en` and `es`, falling back to English. Each context keeps its messages in `<Context>/Resources/<Context>Messages.resx` and `<Context>Messages.es.resx`, keyed by error type and value, such as `RoomError.RoomNotFound`. Only the UI culture changes: numbers and dates are always read in English, so request values such as `120.50` keep their meaning.

### Consequences
- (+) The `detail` of every business error follows the language of the client, while the `title` stays a stable code (ADR-004).
- (−) Messages of individual validation fields and of malformed parameters remain in English.

---

## ADR-017 — Seed demonstration data through the application services

- **Status:** Accepted
- **Date:** 2026-11-10

### Context
The frontend and the course demonstration need a realistic hotel: stays from the last weeks, guests staying now, today's arrivals, payments, room statuses, inventory, and credentials, all placed around the current day.

### Decision
`DemoDataSeeder` runs after the database is created when `Hostera:SeedDemoData` is `true`, the default in Development, and only when the database has no room types. It creates the data through the same command services as the API, so every business rule applies, and sets only past stay events directly on the aggregates.

### Consequences
- (+) The demonstration data is always valid and relative to the day the database is created.
- (−) Seeding takes a few seconds on the first start, and a failing step is logged instead of stopping the API.

---

## ADR-018 — Deploy a Docker container on Render with a managed MySQL database

- **Status:** Accepted
- **Date:** 2026-11-10

### Context
The frontend is published on Firebase Hosting and needs a public API. Render hosts the team's previous demonstration API, but it has no native .NET runtime and offers no managed MySQL. A MySQL container on Render would need a paid private service and persistent disk, and its backups would be the team's responsibility.

### Decision
Build the API with a multi-stage `Dockerfile` (.NET 10 SDK to build, ASP.NET Core 10 runtime to run, port `8080`) and describe the Render web service in `render.yaml`. Host the database on a free Aiven for MySQL service with an encrypted connection, whose connection string is stored as a Render secret.

### Consequences
- (+) The same image runs locally and on Render, and every push to `main` redeploys it.
- (+) The database persists across deployments without maintenance by the team.
- (−) A free Render service sleeps without traffic, so the first request after a pause is slow.
- (−) The free Aiven plan is a single node with limited storage and no availability guarantee, which suits a demonstration but not production customers.
