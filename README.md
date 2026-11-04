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
| Bookings | Bookings of guest stays, their codes, totals, and room availability rules. |
| Inventory Management | Storage locations, inventory items, their stock per location, and stock adjustments. |
| Access Control | RFID credentials for guests and staff, staff members, and access events. |
| Overview | The operational dashboard of a property and the comparison of properties. It only reads other contexts. |

Booking status changes, payments, check-in, and check-out are still in development.

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
| Rooms | `PUT /rooms/{roomId}/status` | Set or release the operational status of a room over a date range. |
| Rooms | `GET /properties/{propertyId}/status-periods` | List the status periods of the rooms of a property, optionally between `from` and `to`. |
| Rooms | `GET /properties/{propertyId}/room-availability?startDate=` | Get the status of each room for seven days, optionally filtered by `status`. |
| Rooms | `GET /properties/{propertyId}/rate-plans` | List the rate plans of a property, with its currency. |
| Rooms | `POST /properties/{propertyId}/rate-plans` | Create a rate plan. |
| Rooms | `GET /rate-plans/{ratePlanId}` | Get a rate plan. |
| Rooms | `PUT /rate-plans/{ratePlanId}` | Update a rate plan; plans are made inactive instead of deleted. |
| Rooms | `PUT /rate-plans/{ratePlanId}/daily-rates` | Set the price of a range of nights for a room type, or return them to the base rate. |
| Rooms | `GET /properties/{propertyId}/daily-rates?startDate=&endDate=` | List the daily rates of a property between two nights. |
| Bookings | `GET /properties/{propertyId}/bookings` | List the bookings of a property, filtered by `status`, `roomId`, `from`, `to`, and `search`, and optionally paged with `page` and `pageSize` (`X-Total-Count` header). |
| Bookings | `POST /properties/{propertyId}/bookings` | Create a pending booking; the server assigns its code and total. |
| Bookings | `GET /bookings/{bookingId}` | Get a booking. |
| Bookings | `PUT /bookings/{bookingId}` | Update a pending or confirmed booking. |
| Inventory | `GET`, `POST /properties/{propertyId}/storage-locations` | List and create storage locations. |
| Inventory | `GET`, `PUT`, `DELETE /storage-locations/{storageLocationId}` | Get, update, and delete a storage location. |
| Inventory | `GET`, `POST /properties/{propertyId}/inventory-items` | List and create inventory items. |
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
| Overview | `GET /properties/{propertyId}/dashboard` | Get the operational dashboard of a property. |
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

In the Development environment, Scalar is available at:

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

## Error responses

Errors use the RFC 9457 problem details format. The `title` holds a stable error code, such as `DuplicateRoomTypeName`, and `detail` explains it:

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
- **409 Conflict**: the request breaks a business rule, such as a duplicate name or insufficient stock.

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
