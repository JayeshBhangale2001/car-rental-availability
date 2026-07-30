# Car Rental Availability

Offline, deterministic car rental availability API built with .NET 8 Minimal API.

## Project Overview

This solution implements:
- Car search across two stub providers
- Booking creation with location/document business rules
- Booking lookup by reference

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

## Setup

Prerequisites:
- .NET SDK 8.0+

Run locally:

```bash
cd src
dotnet restore CarRental.sln
dotnet run --project CarRental.Api
```

Default local URLs are shown by ASP.NET at startup. Swagger UI is enabled.

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

### 1) Search Cars

- Method/Path: `GET /cars/search`
- Query:
	- Required: `pickup`, `from`, `to`
	- Optional: `category` (`Economy|Compact|SUV|Minivan`)
- Success: `200 OK`
- Main errors: `400 Bad Request`

### 2) Create Booking

- Method/Path: `POST /cars/book`
- Body includes selected offer context plus driver/document/rental details
- Success: `201 Created`
- Main errors:
	- `400 Bad Request` for invalid input/search context
	- `422 Unprocessable Entity` for document rule mismatch

Document business rule:
- Domestic pickup requires `NationalId`
- International pickup requires `Passport`

### 3) Get Booking By Reference

- Method/Path: `GET /cars/booking/{reference}`
- Success: `200 OK`
- Main errors: `404 Not Found` (unknown reference), `400 Bad Request` (empty reference)

## Sample Requests

Search:

```bash
curl "http://localhost:5000/cars/search?pickup=Mumbai&from=2026-07-01&to=2026-07-04&category=Economy"
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
