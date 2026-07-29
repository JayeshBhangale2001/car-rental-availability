# Car Rental Availability Specification

## 1. Overview and Scope

This specification defines the implementation blueprint for the Car Rental Availability feature for SkyRoute.

### In Scope
- Backend API using .NET 8+ Minimal API with:
  - Car search across two stub providers
  - Booking creation with document validation
  - Booking lookup by reference
- Frontend flow for:
  - Search input
  - Results display and sorting
  - Booking submission
  - Booking confirmation display
- Provider normalization and provider-specific pricing rules
- Deterministic, fully offline operation on a local machine

### Out of Scope
- Real rental provider APIs
- Authentication/authorization
- External credentials or secrets
- Database persistence requirements

## 2. Functional Requirements

### 2.1 Search and Provider Aggregation
- The system shall query both providers and return a normalized list of available vehicles.
- Supported categories are Economy, Compact, SUV, and Minivan.
- Endpoint:
  - `GET /cars/search?pickup={location}&from={date}&to={date}&category={category}`
- Query parameters:
  - Required: pickup, from, to
  - Optional: category
- Each result shall include:
  - provider
  - vehicle category
  - per-day rate (INR)
  - total rental price (INR)
  - cancellation policy
  - insurance indicator
- BudgetWheels entries returned as unavailable shall be filtered out.

### 2.2 Pricing
- PremiumDrive uses flat daily rate pricing.
- BudgetWheels uses base daily rate with weekend surcharge.
- Both per-day rate and total price must be returned.

### 2.3 Booking
- Endpoint:
  - `POST /cars/book`
- The system shall validate booking documents against pickup location type.
- On success, booking shall return a reference number and confirmation details.

### 2.4 Booking Lookup
- Endpoint:
  - `GET /cars/booking/{reference}`
- The system shall return booking details for an existing reference.

### 2.5 Frontend States and Sorting
- Search form fields:
  - pickup location
  - pickup date
  - return date
  - optional category
- Results view shall show required vehicle and policy information.
- Search results are displayed sorted by total price in ascending order.
- Booking form fields:
  - driver name
  - document type
  - document number
- Confirmation view shall show:
  - reference number
  - provider
  - total price
  - cancellation policy
- UI shall handle results, empty, error, and confirmation states.

## 3. Assumptions and Business Rules

### 3.1 Assumptions
- Dates use calendar format only (`YYYY-MM-DD`).
- No timezone conversion is performed.
- Prices are consistently represented in Indian Rupees (INR).
- Pickup location matching is case-insensitive after trimming whitespace.
- Unsupported category returns HTTP 400 with a clear validation message.
- Unsupported pickup location returns HTTP 400 with a clear validation message.
- Domestic bookings require National ID.
- International bookings require Passport.

### 3.2 Business Rules
- Rental nights are counted from pickup date inclusive to return date exclusive.
- Return date must be strictly after pickup date.
- BudgetWheels weekend surcharge applies only to BudgetWheels.
- Weekend surcharge is applied per rental night based on each night start date.
- Friday, Saturday, and Sunday nights are charged at 20% above weekday rate.
- BudgetWheels total is calculated by iterating rental nights, not by simple multiplication.
- PremiumDrive is always available.
- BudgetWheels may return unavailable vehicles; these must be excluded from results.

## 4. Domain and Provider Design

### 4.1 Core Domain Concepts
- VehicleCategory: Economy, Compact, SUV, Minivan
- Provider: PremiumDrive, BudgetWheels
- SearchRequest: pickup, from, to, optional category
- SearchResult (normalized): provider, category, perDayRate, totalPrice, cancellationPolicy, insurance indicator
- Booking: reference, selected offer context, driver details, document details, pickup/rental details, totalPrice, cancellationPolicy

### 4.2 Provider Abstraction
- Providers implement a shared abstraction (`ICarRentalProvider`) and are injected through DI.
- Each provider implementation returns provider-specific search data.
- Core flow responsibilities:
  - Call all registered providers
  - Apply provider pricing logic
  - Filter unavailable entries
  - Normalize output to one response model
- Extensibility goal:
  - A third provider can be added by implementing the provider abstraction without reworking the core orchestration flow.

### 4.3 Booking Storage Model
- Bookings are stored in memory for the application lifetime.
- This supports booking lookup by reference without database persistence.

## 5. API Contracts and Validation

### 5.1 GET /cars/search
Query parameters:
- pickup (required)
- from (required, `YYYY-MM-DD`)
- to (required, `YYYY-MM-DD`)
- category (optional, must be one of Economy/Compact/SUV/Minivan if present)

Success:
- Returns normalized list of available vehicles with required result fields.

Validation and errors:
- HTTP 400 when pickup, from, or to is missing.
- HTTP 400 when to is not after from.
- HTTP 400 when category is invalid.
- HTTP 400 when pickup location is unsupported.
- Unavailable BudgetWheels entries are not included in success results.

### 5.2 POST /cars/book
Request body includes:
- Selected offer/provider context
- driverName
- documentType
- documentNumber
- pickup location and rental dates

Success:
- Returns booking confirmation with reference number, provider, total price, and cancellation policy.

Validation and errors:
- Server enforces document rules:
  - Domestic pickup -> National ID
  - International pickup -> Passport
- HTTP 422 with clear message on document mismatch.

### 5.3 GET /cars/booking/{reference}
Path parameter:
- reference

Success:
- Returns booking details when reference exists.

Errors:
- HTTP 404 when booking reference is not found.

## 6. Frontend Behaviour

### 6.1 Search
- User enters pickup location, pickup date, return date, and optional category.
- Client-side validation mirrors server validation for required fields and date ordering.
- On submit, frontend calls `GET /cars/search` and renders one of:
  - results state
  - empty state
  - error state

### 6.2 Results
- Each result shows:
  - provider badge
  - vehicle category
  - per-day rate (INR)
  - total price (INR)
  - cancellation policy
  - insurance indicator
- Results are displayed sorted by total price in ascending order.

### 6.3 Booking
- User selects a result and enters:
  - driver name
  - document type
  - document number
- Client-side document validation mirrors server rule:
  - domestic -> National ID
  - international -> Passport
- On submit, frontend calls `POST /cars/book` and handles:
  - validation error display (including 422)
  - success confirmation state

### 6.4 Confirmation
- Confirmation shows:
  - booking reference number
  - provider
  - total price (INR)
  - cancellation policy

## 7. Stub Data and Test Strategy

### 7.1 Stub Data
- Two deterministic stubs: PremiumDrive and BudgetWheels.
- Stub scenarios cover:
  - all supported categories
  - available and unavailable BudgetWheels vehicles
  - date ranges spanning weekday and weekend nights
- Data is stable across runs for repeatable tests and demos.

### 7.2 Test Strategy
- Unit tests cover core business logic:
  - rental night counting (from inclusive, to exclusive)
  - weekend surcharge application by night start date
  - PremiumDrive flat daily pricing behavior
  - filtering unavailable BudgetWheels entries
  - category and location validation behavior
  - document validation behavior (domestic/international)
- API behavior tests cover:
  - search input validation and date validation
  - booking 422 on document mismatch
  - successful booking response contents
  - booking lookup success and 404 not-found behavior
- Frontend behavior checks cover:
  - results, empty, error, confirmation states
  - ascending sort by total price
  - client/server validation consistency in booking flow

## 8. Non-Functional Requirements

- The application runs fully offline from a clean clone using README instructions.
- Behavior is deterministic for reliable local demo and test execution.
- No external credentials or secrets are required or committed.
- Architecture maintains clear separation and SOLID-based design between:
  - API layer
  - provider implementations
  - pricing logic
  - booking logic
  - frontend
- Provider design remains extensible for adding new providers without reworking core flow.
- Submission includes complete backend, frontend, tests, and documentation artifacts required by the challenge.
