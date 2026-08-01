# Car Rental Availability

Offline, deterministic car rental availability API built with .NET 8 Minimal API.

## Project Overview

This solution implements:
- Car search across two stub providers
- Booking creation with location/document business rules
- Booking lookup by reference
- Angular frontend for car search, booking, confirmation, and booking lookup

Current backend scope is fully in-memory and does not require external services.

## Architecture

Repository layout:
- `src/CarRental.Api`: Minimal API host, endpoint contracts, route metadata, Swagger/OpenAPI
- `src/CarRental.Core`: Domain models, validators, providers, pricing, services, in-memory booking storage
- `src/CarRental.Tests`: Unit and integration tests (including WebApplicationFactory endpoint tests)
- `ui/`: Frontend workspace

Layer responsibilities:
- API layer: request parsing, HTTP status mapping, response contracts
- Core layer: validation, provider orchestration, pricing logic, booking rules, storage
- Providers: `PremiumDrive` and `BudgetWheels` stub inventories
- Frontend (`ui/`): Angular UI flow for search, offer selection, booking, confirmation, and lookup

## Setup

For a full Windows new-machine setup and troubleshooting guide, see `NEW_MACHINE_SETUP.md`.

Prerequisites:
- .NET SDK 8.0+
- Node.js 18+ and npm

Run locally:

```bash
# backend
cd src
dotnet restore CarRental.sln
dotnet run --project CarRental.Api

# frontend (new terminal)
cd ../ui
npm install
npm start
```

Default local URLs are shown by ASP.NET at startup. Swagger UI is enabled.
Angular dev server runs on `http://localhost:4200` by default.

## Frontend

Frontend overview:
- Location: `ui/`
- Purpose: thin UI over the existing backend contracts, with no additional business features
- API models are reused from backend response/request shapes in `ui/src/app/core/models/car-rental.models.ts`

Supported UI flow:
- Search form with supported pickup location selection
- Available offers (sorted by total price ascending)
- Select offer
- Booking form
- Confirmation with reference
- Booking lookup

Frontend technology:
- Angular 18 (standalone components)
- Reactive Forms for input handling and client-side validation
- HttpClient for API integration

Local API/proxy behavior:
- Frontend calls relative `/cars/*` API paths
- `ui/proxy.conf.json` proxies `/cars` to `http://localhost:5000`
- `environment.apiBaseUrl` is empty (`''`) for local proxy-based calls
- If backend runs on a different URL/port, update `ui/proxy.conf.json` target

Pickup location discoverability:
- Frontend loads supported pickup locations from `GET /cars/pickup-locations`
- Search uses a guided dropdown grouped by Domestic and International
- This avoids trial-and-error for unsupported locations

## Supported Pickup Locations

- Domestic: Mumbai, Delhi
- International: Dubai, London, Singapore

Matching is case-insensitive after trimming input.

## Pricing Rules

- Currency: INR
- Rental nights: pickup date inclusive, return date exclusive
- `PremiumDrive`: flat daily rate `total = baseRate * nights`
- `BudgetWheels`: base daily rate with 20% surcharge on Friday, Saturday, Sunday nights
- `BudgetWheels` unavailable offers are filtered from search results

## API Endpoints

### 1) Get Supported Pickup Locations

- Method/Path: `GET /cars/pickup-locations`
- Success: `200 OK`
- Response: list of `{ name, locationType }`

### 2) Search Cars

- Method/Path: `GET /cars/search`
- Query:
  - Required: `pickup`, `from`, `to`
  - Optional: `category` (`Economy|Compact|SUV|Minivan`)
- Success: `200 OK`
- Main errors: `400 Bad Request`

### 3) Create Booking

- Method/Path: `POST /cars/book`
- Body includes selected offer context plus driver/document/rental details
- Success: `201 Created`
- Main errors:
  - `400 Bad Request` for invalid input/search context
  - `422 Unprocessable Entity` for document rule mismatch

Document business rule:
- Domestic pickup requires `NationalId`
- International pickup requires `Passport`

### 4) Get Booking By Reference

- Method/Path: `GET /cars/booking/{reference}`
- Success: `200 OK`
- Main errors: `404 Not Found` (unknown reference), `400 Bad Request` (empty reference)

## Sample Requests

Search:

```bash
curl "http://localhost:5000/cars/search?pickup=Mumbai&from=2026-07-01&to=2026-07-04&category=Economy"
```

Pickup locations:

```bash
curl "http://localhost:5000/cars/pickup-locations"
```

Book:

```bash
curl -X POST "http://localhost:5000/cars/book" \
  -H "Content-Type: application/json" \
  -d '{
    "provider": "PremiumDrive",
    "offerId": "PD-ECON-001",
    "driverName": "Jayesh",
    "documentType": "NationalId",
    "documentNumber": "NID-12345",
    "pickup": "Mumbai",
    "from": "2026-07-01",
    "to": "2026-07-04"
  }'
```

Lookup:

```bash
curl "http://localhost:5000/cars/booking/BK-EXAMPLE123"
```

## Assumptions

- Date format is strict `YYYY-MM-DD`
- Return date must be strictly after pickup date
- No authentication or authorization
- No external provider APIs
- In-memory booking persistence for application lifetime only
- Fully offline execution from clean clone

## Run Tests

```bash
cd src
dotnet test CarRental.sln -v minimal
```

Frontend tests:

```bash
cd ui
npm run test:ci
```

Notes:
- `test:ci` runs Angular unit tests once in headless mode using `ng test --watch=false --browsers=ChromeHeadless --progress=false`
- On machines without Google Chrome installed, set `CHROME_BIN` to a Chromium-based browser such as Microsoft Edge before running the command

Example on Windows:

```powershell
$env:CHROME_BIN = 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'
cd ui
npm run test:ci
```
