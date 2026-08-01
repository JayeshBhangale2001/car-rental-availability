# Copilot Instructions for Car Rental Availability

This repository uses a layered architecture. Keep code changes small, consistent, and within the correct layer.

## Architecture and Responsibilities

- Keep layer boundaries strict:
  - `src/CarRental.Api`: Minimal API hosting, endpoint mapping, request/response DTO mapping, HTTP status translation.
  - `src/CarRental.Core`: Domain models, validation, provider orchestration, pricing rules, booking business rules, storage abstractions.
  - `src/CarRental.Tests`: Unit tests for Core logic and integration tests for API behavior.
  - `ui/`: Angular presentation and API consumption.
- Do not move business rules into API endpoints or Angular components.

## Core-First Business Logic

- Put business behavior in `CarRental.Core` services/validators/providers/pricing classes.
- Keep rules deterministic and offline-friendly.
- Prefer extending existing abstractions (`IValidator<T>`, `ICarRentalProvider`, `IPricingCalculator`, service interfaces) over ad-hoc logic.

## Thin Minimal API Endpoints

- In `CarsEndpoints`, keep handlers thin:
  - Parse and validate request shape.
  - Call Core services.
  - Map results to contracts and HTTP responses.
- Avoid embedding pricing, document rules, or provider-specific branching in endpoint methods.

## Dependency Injection

- Register dependencies in `src/CarRental.Api/Program.cs`.
- Depend on interfaces where available (`ICarSearchService`, `IBookingService`, `IValidator<T>`, `ICarRentalProvider`, storage abstractions).
- Keep service lifetimes consistent with current design unless there is a clear reason to change.

## Naming and Style Conventions

- Follow existing C# naming:
  - `PascalCase` for public types/members.
  - Interface names prefixed with `I`.
  - Async methods suffixed with `Async`.
  - Private readonly fields in `camelCase`.
- Match existing DTO and test naming patterns (e.g., `SearchCarsRequestDto`, `CreateBookingAsync_ValidBooking_SavesBookingAndReturnsConfirmation`).
- Preserve existing provider identifiers and values (`PremiumDrive`, `BudgetWheels`, INR, cancellation policy text) unless the change explicitly requires updates.

## Extending Providers and Pricing

When adding a provider:
- Implement `ICarRentalProvider` in `CarRental.Core/Providers`.
- Add provider-specific vehicle seed data and map to normalized `CarOffer`.
- Keep provider-specific rules in a dedicated pricing calculator that implements `IPricingCalculator`.
- Register the new provider/calculator in DI.
- Ensure search aggregation behavior remains unchanged in `CarSearchService`.

When changing pricing:
- Keep date/night calculations consistent with `RentalNightCalculator`.
- Encode pricing differences in calculator classes, not endpoint code.

## Testing Expectations

- Add or update unit tests in `src/CarRental.Tests/Unit` for Core rule changes.
- Add or update integration tests in `src/CarRental.Tests/Integration` for API contract/status-code behavior.
- Prefer deterministic test data and explicit assertions for error codes/messages.

## Angular UI Guidance

- Keep Angular components presentation-focused and state-oriented.
- Place API access and error parsing in Angular services (for example, `CarRentalApiService`).
- Do not duplicate backend business rules in components; at most mirror validation for UX.
- Keep request/response models aligned with backend contracts.
