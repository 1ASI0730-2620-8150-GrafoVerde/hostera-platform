# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-11-10

### Added

- ASP.NET Core RESTful API on .NET 10, organized by bounded context with Domain, Application, Infrastructure, and Interfaces layers, persisted in MySQL through Entity Framework Core with snake_case naming, audit timestamps, and a transactional unit of work.
- Rooms & Availability: properties, room types, rooms, and rate plans with their room types, included services, and currency; daily rates set or returned to the base nightly rate for a date range in one request; operational status periods set or released for a date range in one request; and the room availability of a property per day.
- Bookings: bookings of a property with filters, search, and paging; server-assigned codes and quoted totals; edits with availability checks; confirmation, cancellation with a reason, no-show, and restoration; payments up to the balance due with idempotency keys; check-in with identity verification that issues the guest key cards; and check-out that ends the key cards and marks the room for cleaning.
- Inventory Management: storage locations, inventory items created with their opening stock, stock per location, stock adjustments and transfers, and the removal of locations an item no longer uses.
- Access Control: guest key cards and staff credentials with their access period, issuing, revocation, and replacement of cards, staff members, and RFID access events recorded by door readers and queried with filters and paging.
- Overview: the operational dashboard of a property, with tonight's occupancy, room statuses, today's arrivals, daily revenue and occupancy, inventory alerts, and access attempts, and the comparison of every property on a day.
- Optimistic concurrency for bookings through the `ETag` and `If-Match` headers.
- Errors as problem details with a stable code in `title`, described in English or Spanish according to the `Accept-Language` header.
- Day-dependent rules evaluated in the time zone of the properties.
- Demonstration hotel loaded into a new database when `Hostera:SeedDemoData` is enabled, and the OpenAPI document with the Scalar reference when `Hostera:PublishApiReference` is enabled; both are enabled by default in Development.
- Dockerfile and Render Blueprint to deploy the API with a MySQL database on Aiven.
- Documentation: README, architecture decision records, class diagram of every bounded context, technical stories, and the MIT license.

[1.0.0]: https://github.com/1ASI0730-2620-8150-GrafoVerde/hostera-platform-2/releases/tag/1.0.0
