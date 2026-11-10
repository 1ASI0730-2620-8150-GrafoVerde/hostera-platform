# Hostera Platform

## Overview

Hostera Platform is the RESTful backend of Hostera, a hotel management solution for independent hotels and small hotel chains. It serves the [Hostera Frontend Web Application](https://github.com/1ASI0730-2620-8150-GrafoVerde/hostera-frontend-wa) with the properties, rooms, rates, inventory, access control, and operational overview of each hotel property.

The API is built with **ASP.NET Core** on **.NET 10**, persists its data in **MySQL** through **Entity Framework Core**, and documents its endpoints with **OpenAPI** and **Scalar**.

## Architecture

The platform follows Domain-Driven Design. Each bounded context lives in its own folder and is split into four layers:

- **Domain**: aggregates, entities, value objects, commands, queries, and repository contracts. It has no dependency on ASP.NET Core or EF Core.
- **Application**: command and query services that coordinate the domain, the error enum of the context, and the anti-corruption services that read other contexts. Services return a `Result<TValue, TError>` instead of throwing for business rule violations.
- **Infrastructure**: EF Core mapping and repositories.
- **Interfaces**: REST controllers, request and response resources, and the assemblers that turn resources into commands and aggregates into resources.

The `Shared` context holds the building blocks used by every context: the base repository, the unit of work, the `Result` pattern, auditable timestamps, the kebab-case route convention, and the snake-case database naming convention.

## Bounded contexts

| Context | Responsibility |
| --- | --- |
| Rooms & Availability | Properties, room types, rooms, operational status periods, room availability, rate plans, and daily rates. |
| Bookings | Bookings of guest stays, their codes, totals, status changes, payments, balances, check-in, and check-out. |
| Inventory Management | Storage locations, inventory items, their stock per location, and stock adjustments. |
| Access Control | RFID credentials for guests and staff, staff members, and access events. |
| Overview | The operational dashboard of a property and the comparison of properties. It only reads other contexts. |

## Endpoints

All routes start with `/api/v1`.

| Context | Method and route | Description |
| --- | --- | --- |
| Rooms | `GET /properties` | List the properties. |
| Rooms | `GET /properties/{propertyId}` | Get a property. |
| Rooms | `GET /properties/{propertyId}/room-types` | List the room types of a property. |
| Rooms | `POST /properties/{propertyId}/room-types` | Create a room type. |
| Rooms | `GET /room-types/{roomTypeId}` | Get a room type. |
| Rooms | `PUT /room-types/{roomTypeId}` | Update a room type and its status. |
| Rooms | `DELETE /room-types/{roomTypeId}` | Delete a room type that no room or rate plan uses. |
| Rooms | `GET /properties/{propertyId}/rooms` | List the rooms of a property. |
| Rooms | `POST /properties/{propertyId}/rooms` | Create a room. |
| Rooms | `GET /rooms/{roomId}` | Get a room. |
| Rooms | `PUT /rooms/{roomId}` | Update the number, room type, and floor of a room. |
| Rooms | `PUT /rooms/{roomId}/status` | Set or release the operational status of a room over a date range. A status cannot cover a night held by a booking. |
| Rooms | `GET /properties/{propertyId}/status-periods` | List the status periods of the rooms of a property, optionally between `from` and `to`. |
| Rooms | `GET /properties/{propertyId}/room-availability?startDate=` | Get the status of each room for seven days (booked, occupied, an operational status, or available), optionally filtered by `status`. |
| Rooms | `GET /properties/{propertyId}/rate-plans` | List the rate plans of a property, with its currency. |
| Rooms | `POST /properties/{propertyId}/rate-plans` | Create a rate plan. |
| Rooms | `GET /rate-plans/{ratePlanId}` | Get a rate plan. |
| Rooms | `PUT /rate-plans/{ratePlanId}` | Update a rate plan; plans are made inactive instead of deleted. |
| Rooms | `PUT /rate-plans/{ratePlanId}/daily-rates` | Set the price of a range of nights for a room type, or return them to the base rate. |
| Rooms | `GET /properties/{propertyId}/daily-rates` | List the daily rates of a property, optionally limited with `startDate` and `endDate`. |
| Bookings | `GET /properties/{propertyId}/bookings` | List the bookings of a property, filtered by `status`, `roomId`, `from`, `to`, and `search`, and optionally paged with `page` and `pageSize` (`X-Total-Count` header). |
| Bookings | `POST /properties/{propertyId}/bookings` | Create a pending booking; the server assigns its code and total. |
| Bookings | `GET /bookings/{bookingId}` | Get a booking. |
| Bookings | `PUT /bookings/{bookingId}` | Update a pending or confirmed booking. |
| Bookings | `POST /bookings/{bookingId}/check-in` | Check in a confirmed booking with a verified identity document and register its key cards in Access Control, valid until 11:00 on the check-out day. |
| Bookings | `POST /bookings/{bookingId}/check-out` | Check out a paid booking, end its key cards, and mark the room as needs-cleaning on the departure day. |
| Bookings | `GET /bookings/{bookingId}/payments` | List the payments of a booking. |
| Bookings | `POST /bookings/{bookingId}/payments` | Record a payment up to the balance due. Send an `Idempotency-Key` header to make retries safe. |
| Bookings | `GET /properties/{propertyId}/payments` | List the payments received for the bookings of a property. |
| Bookings | `POST /bookings/{bookingId}/transitions` | Confirm, cancel, mark as a no-show, or restore a booking. A rejected transition returns the current status and the allowed transitions. |
| Inventory | `GET`, `POST /properties/{propertyId}/storage-locations` | List and create storage locations. |
| Inventory | `GET`, `PUT`, `DELETE /storage-locations/{storageLocationId}` | Get, update, and delete a storage location. |
| Inventory | `GET`, `POST /properties/{propertyId}/inventory-items` | List and create inventory items; an optional `openingQuantity` is recorded with the new item. |
| Inventory | `GET`, `PUT /inventory-items/{inventoryItemId}` | Get and update an inventory item. |
| Inventory | `POST /inventory-items/{inventoryItemId}/stock-adjustments` | Adjust or transfer stock. |
| Inventory | `DELETE /inventory-items/{inventoryItemId}/stocks/{storageLocationId}` | Remove a storage location from an item. |
| Access Control | `GET /properties/{propertyId}/rfid-credentials` | List the RFID credentials of a property. |
| Access Control | `POST /properties/{propertyId}/rfid-credentials/encode/guest-key-cards` | Issue the key cards of a guest. |
| Access Control | `POST /properties/{propertyId}/rfid-credentials/encode/staff-credential` | Issue a staff credential. |
| Access Control | `GET /rfid-credentials/{credentialId}` | Get a credential. |
| Access Control | `POST /rfid-credentials/{credentialId}/revoke` | Revoke a credential. |
| Access Control | `POST /rfid-credentials/{credentialId}/replace` | Replace a credential with a new card. |
| Access Control | `GET /properties/{propertyId}/staff-members` | List the staff members of a property. |
| Access Control | `GET /properties/{propertyId}/access-events` | List the access events of a property, paginated. |
| Access Control | `GET /access-events/{accessEventId}`, `POST /access-events` | Get and record access events. |
| Overview | `GET /properties/{propertyId}/dashboard` | Get the operational dashboard of a property, including today's arrivals with their status and balance due. |
| Overview | `GET /property-overviews` | Compare the properties on a date. |

Scalar lists every endpoint with its request and response resources.

## Technologies

- .NET 10 and ASP.NET Core
- Entity Framework Core 10 with the `MySql.EntityFrameworkCore` provider
- MySQL 8
- OpenAPI with Scalar as the API reference
- Humanizer for table name pluralization

## Getting started

### Prerequisites

- .NET SDK 10
- MySQL 8 running on `localhost:3306`, with a password for the `root` user

### 1) Set the database password and time zone

The connection string in `HosteraBackend.API/appsettings.json` reads the password of the MySQL `root` user from the `HOSTERA_DB_PASSWORD` environment variable, so no credential is stored in the repository:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;user=root;password=%HOSTERA_DB_PASSWORD%;database=hostera"
}
```

Define the variable in your shell profile, for example in `~/.zshrc` or `~/.bashrc`:

```bash
export HOSTERA_DB_PASSWORD='your-mysql-root-password'
```

On Windows, define it as a user environment variable. Restart your terminal or IDE afterward.

Rules that depend on the current day, such as when a booking can be checked in, use the time zone in `Hostera:TimeZone` (`America/Lima` by default).

### 2) Restore and run the API

```bash
dotnet restore hostera-backend.sln
dotnet run --project HosteraBackend.API
```

### 3) Explore the API with Scalar

In the Development environment, or wherever `Hostera:PublishApiReference` is `true`, Scalar is available at:

- `http://localhost:5264/scalar`
- `https://localhost:7240/scalar`

The generated OpenAPI document is available at `/openapi/v1.json`.

### Database initialization

On startup, the API calls `Database.EnsureCreated()`. When the `hostera` database does not exist, it creates the database with every table and seeds the properties and staff members. When the database already exists, it changes nothing.

Because the project does not use migrations yet, **drop the database after pulling changes that add or modify entities**, and let the API create it again:

```sql
DROP DATABASE hostera;
```

This removes the data created through the API.

### Demonstration data

A new database receives a demonstration hotel the first time the API starts when `Hostera:SeedDemoData` is `true`, which is the default in the Development environment. Both properties get the room types, rooms, rate plans, storage locations, inventory items, and staff credentials of the frontend mock data, and their bookings, payments, room statuses, and access events are placed around the current day: stays from the last weeks, guests staying now, today's arrivals, and bookings ahead. A database that already has room types is left unchanged.

To start with an empty database instead, set `Hostera:SeedDemoData` to `false` in `appsettings.Development.json` or with the `Hostera__SeedDemoData=false` environment variable. In other environments, such as the deployed API, set `Hostera__SeedDemoData=true` to load it. To load the demonstration data again, drop the database and restart the API.

## Concurrency

Booking responses carry the version of the booking in the `ETag` header and in the `version` field. To make sure a change does not overwrite someone else's, send that ETag back in `If-Match` when updating a booking, changing its status, recording a payment, or checking in or out:

```http
PUT /api/v1/bookings/12
If-Match: "4"
```

When the booking changed after it was read, the API answers `409 Conflict` with the `ConcurrencyConflict` code and keeps the newer data. Requests without `If-Match` skip the check. Payments always increase the version of their booking, so two payments sent at the same time cannot exceed the balance due.

Every save runs in a database transaction, so a request that fails halfway leaves no partial changes.

Browser clients can read the `ETag`, `X-Total-Count`, and `Idempotent-Replayed` response headers.

## Error responses

Errors use the RFC 9457 problem details format. The `title` holds a stable error code, such as `DuplicateRoomTypeName`, and `detail` explains it in the language of the request:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "DuplicateRoomTypeName",
  "status": 409,
  "detail": "Another room type of the property uses the same name."
}
```

- **400 Bad Request**: a value is missing or not valid.
- **404 Not Found**: the resource, or the property it belongs to, does not exist.
- **409 Conflict**: the request breaks a business rule, such as a duplicate name or insufficient stock, or the booking changed after it was read.
- **422 Unprocessable Content**: a payment amount is not valid or exceeds the balance due.

### Language of the messages

The `detail` is written in English or Spanish, following the `Accept-Language` header of the request. Any other language, or a request without the header, gets English. The `title` never changes with the language, so clients can recognize the error by its code and show their own text:

```http
GET /api/v1/rooms/999 HTTP/1.1
Accept-Language: es
```

```json
{
  "title": "RoomNotFound",
  "status": 404,
  "detail": "No se encontró la habitación indicada."
}
```

- The texts of each bounded context live in `<Context>/Resources/<Context>Messages.resx` (English) and `<Context>Messages.es.resx` (Spanish), keyed by error type and value, such as `RoomError.RoomNotFound`. Shared texts live in `Shared/Resources/CommonMessages`.
- A request body that fails validation returns the `InvalidInput` title with a translated `detail`. The messages of each field in `errors` stay in English.
- Only the texts change with the language. Numbers and dates are always read in the English format, such as `120.50` and `2026-11-20`.

## Project structure

```text
HosteraBackend.API/
  AccessControl/        Access Control bounded context
  Bookings/             Bookings bounded context
  Inventory/            Inventory Management bounded context
  Overview/             Overview bounded context
  Rooms/                Rooms & Availability bounded context
  Shared/               Shared building blocks
    Application/        Result pattern
    Domain/             Auditable entity and repository contracts
    Infrastructure/     EF Core context, conventions, and base repository
  Program.cs            Service registration and HTTP pipeline
docs/
  class-diagram.puml    PlantUML class diagram
  user-stories.md       Technical stories of the API
```

Each bounded context folder contains `Domain/`, `Application/`, `Infrastructure/`, and `Interfaces/`.

## Documentation

- **Technical stories**: [`docs/user-stories.md`](docs/user-stories.md)
- **PlantUML class diagram**: [`docs/class-diagram.puml`](docs/class-diagram.puml)

## Build

```bash
dotnet build hostera-backend.sln
```

## Deployment

The API is published as a Docker container on [Render](https://render.com/), and its MySQL database is hosted on [Aiven for MySQL](https://aiven.io/mysql), because Render does not offer managed MySQL. The `Dockerfile` builds the API with the .NET 10 SDK and runs it on the ASP.NET Core 10 runtime image, which listens on port `8080`.

### Run the container locally

```bash
docker build -t hostera-platform .
docker run --rm -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=server=host.docker.internal;user=root;password=$HOSTERA_DB_PASSWORD;database=hostera" \
  -e Hostera__SeedDemoData=true \
  -e Hostera__PublishApiReference=true \
  hostera-platform
```

On Linux, add `--add-host=host.docker.internal:host-gateway` so the container reaches the MySQL server of the host. The API is then available at `http://localhost:8080/api/v1` and Scalar at `http://localhost:8080/scalar`.

### Create the database on Aiven

1. Create a free **Aiven for MySQL** service in the region closest to the Render service.
2. From the service overview, copy the host, port, user (`avnadmin`), and password.
3. Build the connection string. The API creates the `hostera` database and its tables on the first start, and Aiven requires an encrypted connection:

   ```text
   server=<host>;port=<port>;user=avnadmin;password=<password>;database=hostera;SslMode=Required
   ```

### Deploy to Render

`render.yaml` describes the web service as a Render Blueprint: a free Docker web service built from the `main` branch, with `/api/v1/properties` as its health check.

1. In the Render dashboard, choose **New > Blueprint** and select this repository.
2. When Render asks for `ConnectionStrings__DefaultConnection`, paste the Aiven connection string. It is stored as a secret and never committed.
3. Apply the Blueprint. Render builds the image, starts the container, and redeploys it on every push to `main`.

| Variable | Value | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Runs the API with the production settings. |
| `PORT` | `8080` | Tells Render the port the container listens on. |
| `ConnectionStrings__DefaultConnection` | Aiven connection string | Connects the API to its MySQL database. |
| `Hostera__SeedDemoData` | `true` | Loads the demonstration hotel into the new database. |
| `Hostera__PublishApiReference` | `true` | Publishes the OpenAPI document and the Scalar reference. |

After the deployment, check that `https://<service>.onrender.com/api/v1/properties` returns `200 OK`, and set `VITE_HOSTERA_PLATFORM_API_URL` of the frontend to `https://<service>.onrender.com/api/v1`. A free Render service sleeps after a period without requests, so the first request after a pause can take close to a minute.

