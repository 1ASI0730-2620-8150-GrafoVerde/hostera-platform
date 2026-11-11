# Hostera Platform REST API Technical Stories

## Overview

This document contains the API-focused technical stories of the Hostera Platform, intended for frontend and integration developers. They come from the Product Backlog of the Hostera project report.

All routes start with `/api/v1` and follow the kebab-case route naming convention.

## Implementation status

| Story | Title | Epic | Status | Notes |
| --- | --- | --- | --- | --- |
| [TS001](#ts001--register-an-account-through-the-api) | Register an account through the API | EP002 | Pending |  |
| [TS002](#ts002--authenticate-an-operator-through-the-api) | Authenticate an operator through the API | EP002 | Pending |  |
| [TS003](#ts003--enforce-property-scoped-authorization) | Enforce property-scoped authorization | EP002 | Pending |  |
| [TS004](#ts004--retrieve-the-operational-dashboard-through-the-api) | Retrieve the operational dashboard through the API | EP002 | Implemented | Every section reads its source context; today's arrivals include the booking status and balance due, and a section whose source cannot be read is listed in `unavailableSections`. |
| [TS005](#ts005--query-bookings-through-the-api) | Query bookings through the API | EP003 | Implemented | Bookings are filtered by status, room, stay dates, and guest name or code, and paged with `page`, `pageSize`, and the `X-Total-Count` header. |
| [TS006](#ts006--create-and-update-bookings-through-the-api) | Create and update bookings through the API | EP003 | Implemented | Creation and edits are implemented with server-side codes, totals, and availability. Bookings are updated with `PUT /api/v1/bookings/{bookingId}`, and an `If-Match` header with the booking ETag rejects edits based on a stale version with `409 Conflict`. |
| [TS007](#ts007--transition-booking-status-through-the-api) | Transition booking status through the API | EP003 | Implemented | Confirmation, cancellation with reason, no-show, and restoration are implemented, and a rejected transition returns `409 Conflict` with `currentStatus` and `allowedTransitions`. An `If-Match` header with a stale booking ETag returns `409 Conflict` without overwriting the newer state. |
| [TS008](#ts008--record-payments-through-the-api) | Record payments through the API | EP003 | Implemented | Payments are recorded with `POST /api/v1/bookings/{bookingId}/payments`; the `Idempotency-Key` header returns the original payment on retries, and amounts above the balance due return `422 Unprocessable Content`. Booking resources include the paid amount, balance due, and payment status. |
| [TS009](#ts009--complete-check-in-and-check-out-through-the-api) | Complete check-in and check-out through the API | EP003 | Implemented | Check-in verifies the identity document and registers the key cards in Access Control in the same operation, valid until 11:00 on the check-out day. Check-out requires no balance due, ends the key cards, and marks the room as needs-cleaning on the departure day unless another status or guest already holds that day. |
| [TS010](#ts010--retrieve-room-availability-through-the-api) | Retrieve room availability through the API | EP004 | Implemented | Each day shows Booked or Occupied when a booking holds the room, otherwise its operational status, otherwise Available, and the rooms can be filtered by their status on the start date. |
| [TS011](#ts011--manage-rooms-room-types-and-rates-through-the-api) | Manage rooms, room types, and rates through the API | EP004 | Implemented | Rooms, room types, rate plans, and daily rates are implemented, along with operational status periods. Rate plans include the currency of their property. |
| [TS012](#ts012--manage-inventory-and-stock-adjustments-through-the-api) | Manage inventory and stock adjustments through the API | EP005 | In progress | Items, stock adjustments, and transfers are implemented; the collection has no filters or pagination yet. |
| [TS013](#ts013--manage-storage-locations-through-the-api) | Manage storage locations through the API | EP005 | Implemented | Locations are updated with `PUT /api/v1/storage-locations/{storageLocationId}`. |
| [TS014](#ts014--manage-rfid-credentials-through-the-api) | Manage RFID credentials through the API | EP006 | In progress | Encoding, revocation, and replacement are implemented; encoder device failures are not modeled yet. |
| [TS015](#ts015--record-and-query-rfid-access-events-through-the-api) | Record and query RFID access events through the API | EP006 | Implemented | Access events are filtered by date, result, holder, credential, card, access point, and room, and paginated with `X-Total-Count`. |
| [TS016](#ts016--retrieve-and-export-operational-reports-through-the-api) | Retrieve and export operational reports through the API | EP007 | Pending |  |
| [TS017](#ts017--return-standardized-api-errors) | Return standardized API errors | EP008 | In progress | Business rule errors use problem details with a stable code in `title`; request validation uses the ASP.NET Core validation format. |

## Technical stories

### TS001 — Register an account through the API

As a developer, I want to register an administrator and initial property through the API so that client applications can initialize an authorized hotel operation.

**Epic:** EP002

#### Acceptance criteria

- Scenario: Create a valid registration
    - Given a valid and unique registration payload
    - When the developer sends `POST /api/v1/auth/register`
    - Then the API returns `201 Created` with account, initial property, and authorization identifiers.
- Scenario: Reject a duplicate email
    - Given the submitted work email already belongs to an account
    - When the developer sends `POST /api/v1/auth/register`
    - Then the API returns `409 Conflict` with a stable error code and does not create duplicate resources.
- Scenario: Reject invalid registration data
    - Given the registration payload is incomplete or invalid
    - When the developer sends `POST /api/v1/auth/register`
    - Then the API returns `400 Bad Request` with validation details.

---

### TS002 — Authenticate an operator through the API

As a developer, I want to authenticate an operator and refresh an authorized session through the API so that clients can access protected hotel resources securely.

**Epic:** EP002

#### Acceptance criteria

- Scenario: Authenticate valid credentials
    - Given an active operator account and valid credentials
    - When the developer sends `POST /api/v1/auth/sessions`
    - Then the API returns `200 OK` with an access token, refresh information, expiration, and authorized account context.
- Scenario: Reject invalid credentials
    - Given the submitted credentials do not match an active account
    - When the developer sends `POST /api/v1/auth/sessions`
    - Then the API returns `401 Unauthorized` without identifying which credential failed.
- Scenario: Refresh an active session
    - Given a valid refresh credential
    - When the developer sends `POST /api/v1/auth/sessions/refresh`
    - Then the API returns `200 OK` with renewed session credentials and invalidates the replaced refresh credential.

---

### TS003 — Enforce property-scoped authorization

As a developer, I want protected API resources to enforce property-scoped authorization so that one hotel operation cannot access another property's data.

**Epic:** EP002

#### Acceptance criteria

- Scenario: Allow an authorized property request
    - Given the authenticated operator has the required permission for the requested property
    - When the developer requests a protected property resource
    - Then the API processes the request within that property scope.
- Scenario: Deny an unauthorized property request
    - Given the authenticated operator is not authorized for the requested property
    - When the developer requests a protected property resource
    - Then the API returns `403 Forbidden` without exposing the property's protected data.
- Scenario: Reject a missing authentication credential
    - Given no valid access credential accompanies a protected request
    - When the developer requests the resource
    - Then the API returns `401 Unauthorized`.

---

### TS004 — Retrieve the operational dashboard through the API

As a developer, I want to retrieve a property-scoped operational overview through the API so that clients can present current hotel conditions.

**Epic:** EP002

#### Acceptance criteria

- Scenario: Retrieve a current overview
    - Given the requester is authorized for the property
    - When the developer sends `GET /api/v1/properties/{propertyId}/dashboard`
    - Then the API returns `200 OK` with booking, occupancy, room, inventory, access, and upcoming-arrival summaries.
- Scenario: Return a partial-data condition
    - Given one contributing service cannot provide current data
    - When the developer requests the dashboard
    - Then the API returns the available overview data with a machine-readable indication of the unavailable section.

---

### TS005 — Query bookings through the API

As a developer, I want to query property bookings with supported criteria so that clients can retrieve the required subset predictably.

**Epic:** EP003

#### Acceptance criteria

- Scenario: Retrieve a paginated booking collection
    - Given the requester is authorized for the property
    - When the developer sends `GET /api/v1/properties/{propertyId}/bookings`
    - Then the API returns `200 OK` with a paginated collection and booking summary fields.
- Scenario: Apply booking criteria
    - Given valid date, status, guest, room, or search criteria are provided
    - When the developer requests the booking collection
    - Then the API returns `200 OK` with records satisfying all supplied criteria.
- Scenario: Reject invalid query criteria
    - Given one or more query values are invalid
    - When the developer requests the booking collection
    - Then the API returns `400 Bad Request` with validation details.

---

### TS006 — Create and update bookings through the API

As a developer, I want to create and update bookings through the API so that clients can maintain valid guest stays without conflicting room assignments.

**Epic:** EP003

#### Acceptance criteria

- Scenario: Create a valid booking
    - Given a valid guest, stay period, room, and rate payload
    - When the developer sends `POST /api/v1/properties/{propertyId}/bookings`
    - Then the API returns `201 Created` with the booking resource, unique code, calculated total, and initial statuses.
- Scenario: Update a modifiable booking
    - Given the booking exists and the proposed changes satisfy availability and lifecycle rules
    - When the developer sends `PATCH /api/v1/bookings/{bookingId}`
    - Then the API returns `200 OK` with the updated booking and recalculated values.
- Scenario: Reject a room conflict
    - Given the requested room is already assigned during the proposed period
    - When the developer creates or updates the booking
    - Then the API returns `409 Conflict` and preserves existing assignments.

---

### TS007 — Transition booking status through the API

As a developer, I want to execute explicit booking-status transitions through the API so that clients cannot bypass lifecycle rules.

**Epic:** EP003

#### Acceptance criteria

- Scenario: Execute an allowed transition
    - Given the requested transition is valid from the booking's current status
    - When the developer sends `POST /api/v1/bookings/{bookingId}/transitions` with the target status and required reason
    - Then the API returns `200 OK` with the updated status and transition record.
- Scenario: Reject an invalid transition
    - Given the requested transition is not valid from the current status
    - When the developer submits the transition
    - Then the API returns `409 Conflict` with the current status and allowed transitions.
- Scenario: Protect concurrent updates
    - Given the booking changed after the client retrieved it
    - When the developer submits a transition using stale concurrency information
    - Then the API returns `409 Conflict` without overwriting the newer state.

---

### TS008 — Record payments through the API

As a developer, I want to record payments against bookings through the API so that clients can maintain paid amounts and balances without duplicating records.

**Epic:** EP003

#### Acceptance criteria

- Scenario: Record a valid payment
    - Given a booking with an outstanding balance and a valid payment payload
    - When the developer sends `POST /api/v1/bookings/{bookingId}/payments` with an idempotency key
    - Then the API returns `201 Created` with the payment record and updated booking balance.
- Scenario: Repeat an idempotent request
    - Given a payment was already created with the submitted idempotency key
    - When the developer repeats the same request
    - Then the API returns the original successful result without creating another payment.
- Scenario: Reject an invalid amount
    - Given the amount violates the booking's payment rules
    - When the developer submits the payment
    - Then the API returns `422 Unprocessable Content` and preserves the previous balance.

---

### TS009 — Complete check-in and check-out through the API

As a developer, I want to complete check-in and check-out through explicit API operations so that stay, room, payment, and credential states change consistently.

**Epic:** EP003

#### Acceptance criteria

- Scenario: Complete valid check-in
    - Given the booking satisfies identity, payment, room, and credential requirements
    - When the developer sends `POST /api/v1/bookings/{bookingId}/check-in`
    - Then the API returns `200 OK` with the checked-in booking, occupied room, and active-stay information.
- Scenario: Reject incomplete check-in
    - Given one or more required check-in conditions remain unresolved
    - When the developer submits the check-in request
    - Then the API returns `409 Conflict` with the unresolved conditions.
- Scenario: Complete valid check-out
    - Given the active stay satisfies the check-out conditions
    - When the developer sends `POST /api/v1/bookings/{bookingId}/check-out`
    - Then the API returns `200 OK` with the checked-out booking, ended credentials, and room status set to needs cleaning.

---

### TS010 — Retrieve room availability through the API

As a developer, I want to retrieve room availability from a selected date through the API so that clients can plan assignments using consistent daily statuses.

**Epic:** EP004

#### Acceptance criteria

- Scenario: Retrieve a seven-day room period
    - Given the requester is authorized for the property and supplies a valid start date
    - When the developer sends `GET /api/v1/properties/{propertyId}/room-availability?startDate={date}`
    - Then the API returns `200 OK` with the selected date and following six days for each matching room.
- Scenario: Filter by the selected day's status
    - Given a supported room status is supplied
    - When the developer requests room availability
    - Then the API returns rooms whose status on the start date matches the criterion.
- Scenario: Reject an invalid date
    - Given the start date is invalid or outside the supported planning range
    - When the developer requests room availability
    - Then the API returns `400 Bad Request`.

---

### TS011 — Manage rooms, room types, and rates through the API

As a developer, I want to manage rooms, room types, rate plans, and daily prices through the API so that clients can maintain consistent accommodation and pricing configuration.

**Epic:** EP004

#### Acceptance criteria

- Scenario: Create a unique room
    - Given a valid room payload with a unique number and existing room type
    - When the developer sends `POST /api/v1/properties/{propertyId}/rooms`
    - Then the API returns `201 Created` with the room resource.
- Scenario: Create a room type
    - Given a valid and unique room-type payload
    - When the developer sends `POST /api/v1/properties/{propertyId}/room-types`
    - Then the API returns `201 Created` with capacity, bed, base-rate, and status information.
- Scenario: Create a rate plan
    - Given a valid rate-plan payload and applicable room types
    - When the developer sends `POST /api/v1/properties/{propertyId}/rate-plans`
    - Then the API returns `201 Created` with its currency, conditions, services, and applicability.
- Scenario: Set daily room-type prices
    - Given the rate plan, room type, and dates are valid
    - When the developer sends `PUT /api/v1/rate-plans/{ratePlanId}/daily-rates`
    - Then the API returns `200 OK` with the resulting prices and effective dates.

---

### TS012 — Manage inventory and stock adjustments through the API

As a developer, I want to manage inventory records and append stock adjustments through the API so that clients can maintain auditable quantities.

**Epic:** EP005

#### Acceptance criteria

- Scenario: Retrieve filtered inventory
    - Given the requester is authorized for the property
    - When the developer sends `GET /api/v1/properties/{propertyId}/inventory-items` with supported criteria
    - Then the API returns `200 OK` with a paginated inventory collection and calculated stock conditions.
- Scenario: Create an inventory item
    - Given a valid item payload with a unique code and existing storage location
    - When the developer sends `POST /api/v1/properties/{propertyId}/inventory-items`
    - Then the API returns `201 Created` with the inventory item resource.
- Scenario: Append a stock adjustment
    - Given a valid item, location, operation, quantity, and reason
    - When the developer sends `POST /api/v1/inventory-items/{itemId}/stock-adjustments`
    - Then the API returns `201 Created` with the immutable adjustment and resulting quantity.
- Scenario: Prevent negative stock
    - Given a stock-out request exceeds the available quantity
    - When the developer submits the adjustment
    - Then the API returns `409 Conflict` and preserves the current quantity.

---

### TS013 — Manage storage locations through the API

As a developer, I want to manage property storage locations through the API so that inventory items and movements remain associated with valid operational areas.

**Epic:** EP005

#### Acceptance criteria

- Scenario: Create a storage location
    - Given a valid and unique location payload
    - When the developer sends `POST /api/v1/properties/{propertyId}/storage-locations`
    - Then the API returns `201 Created` with the storage-location resource.
- Scenario: Update a storage location
    - Given the location exists and the requester is authorized
    - When the developer sends `PATCH /api/v1/storage-locations/{locationId}`
    - Then the API returns `200 OK` with the updated location while preserving movement history.
- Scenario: Reject removal of a location in use
    - Given items or stock remain assigned to the location
    - When the developer sends `DELETE /api/v1/storage-locations/{locationId}`
    - Then the API returns `409 Conflict` and preserves the location.

---

### TS014 — Manage RFID credentials through the API

As a developer, I want to encode, retrieve, replace, and revoke RFID credentials through the API so that clients and compatible encoders maintain controlled access.

**Epic:** EP006

#### Acceptance criteria

- Scenario: Encode an authorized credential
    - Given a valid person, access scope, validity period, compatible encoder, and blank card
    - When the developer sends `POST /api/v1/properties/{propertyId}/rfid-credentials/encode`
    - Then the API returns `201 Created` with the credential reference, assignment, scope, validity, and active status.
- Scenario: Report an encoder failure
    - Given the selected encoder or blank card cannot complete the operation
    - When the developer submits the encoding request
    - Then the API returns `503 Service Unavailable` with a machine-readable device condition and does not activate the credential.
- Scenario: Revoke a credential
    - Given an active credential and a valid revocation reason
    - When the developer sends `POST /api/v1/rfid-credentials/{credentialId}/revoke`
    - Then the API returns `200 OK` with the revoked status and audit information.
- Scenario: Replace a revoked credential
    - Given the previous credential is revoked and a compatible blank card is available
    - When the developer submits a replacement encoding request
    - Then the API returns `201 Created` with a new credential linked to the replacement history.

---

### TS015 — Record and query RFID access events through the API

As a developer, I want to record and query RFID access events through the API so that authorized clients can investigate granted and denied access attempts.

**Epic:** EP006

#### Acceptance criteria

- Scenario: Record an access event
    - Given a registered access point submits a valid credential attempt
    - When the developer sends `POST /api/v1/access-events`
    - Then the API returns `201 Created` with the immutable timestamp, credential, access point, result, and reason information.
- Scenario: Query property access events
    - Given the requester is authorized for the property
    - When the developer sends `GET /api/v1/properties/{propertyId}/access-events` with supported criteria
    - Then the API returns `200 OK` with a paginated collection satisfying the date, result, person, credential, and access-point criteria.
- Scenario: Reject an unregistered access point
    - Given the event source is not registered or authorized
    - When the developer attempts to record an event
    - Then the API returns `403 Forbidden` and does not create the event.

---

### TS016 — Retrieve and export operational reports through the API

As a developer, I want to retrieve and export property-scoped operational reports through the API so that clients can analyze and share consistent results.

**Epic:** EP007

#### Acceptance criteria

- Scenario: Retrieve a calculated report
    - Given the requester is authorized and provides a supported report type and valid period
    - When the developer sends `GET /api/v1/properties/{propertyId}/reports/{reportType}`
    - Then the API returns `200 OK` with the report scope, generation time, measures, comparison values when applicable, and supporting data.
- Scenario: Export the current report scope
    - Given a supported export format and valid report criteria
    - When the developer sends `GET /api/v1/properties/{propertyId}/reports/{reportType}/export`
    - Then the API returns `200 OK` with the exported file and content type for the same authorized scope.
- Scenario: Reject an unsupported report or format
    - Given the requested report type or export format is not supported
    - When the developer requests the report or export
    - Then the API returns `400 Bad Request` with the supported values.

---

### TS017 — Return standardized API errors

As a developer, I want the API to return consistent validation and domain-error responses so that client applications can handle failures predictably.

**Epic:** EP008

#### Acceptance criteria

- Scenario: Return validation details
    - Given a request contains invalid input
    - When the developer sends the request
    - Then the API returns `400 Bad Request` using a consistent problem-details structure with field-level information when applicable.
- Scenario: Return a missing-resource error
    - Given an authorized request targets a resource that does not exist
    - When the developer sends the request
    - Then the API returns `404 Not Found` using the same problem-details structure.
- Scenario: Return a domain-conflict error
    - Given a valid request violates a current business-state rule
    - When the developer sends the request
    - Then the API returns `409 Conflict` with a stable error code and actionable context.
