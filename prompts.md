# AI Prompt Log

This document records the key AI interactions used during the solution design and implementation process.

---

## 1. Requirement Analysis

### Objective
Understand the case study before writing any code.

### Prompt
Please read the attached case study.

Before we start implementing, help me understand:
- What features need to be built?
- What business rules should I be careful about?
- Are there any requirements that appear ambiguous?

Do not generate code yet.

### Outcome
- Identified required API endpoints.
- Identified frontend flow.
- Identified provider-specific pricing rules.
- Listed initial ambiguities for review.

---

## 2. Requirement Clarification

### Objective
Identify only genuine ambiguities and agree on implementation assumptions.

### Prompt
Please identify only the genuine ambiguities in the case study.

Do not include implementation decisions.

For each ambiguity, suggest a reasonable assumption that can be documented in the specification.

### Outcome
Created an implementation assumptions list covering:
- Date handling
- Rental duration
- Weekend surcharge
- Currency
- Pickup location validation
- Document validation

---

## 3. Assumption Review

### Objective
Refine the assumptions before creating the specification.

### Prompt
Update the assumptions with the following decisions:

- Use Indian Rupees (INR) consistently throughout the application.
- Domestic bookings require National ID.
- International bookings require Passport.

### Outcome
Produced the final implementation assumptions used in the specification.

---

## 4. Specification Planning

### Objective
Define the structure of spec.md before writing it.

### Prompt
Suggest a concise structure for spec.md suitable for this challenge.

The specification should act as an implementation blueprint and remain concise.

Do not generate the document yet.

### Outcome
Defined an eight-section specification covering:
- Overview
- Functional requirements
- Business rules
- Domain design
- API contracts
- Frontend behaviour
- Testing
- Non-functional requirements

---

## 5. Specification Drafting

### Objective
Generate the specification incrementally.

### Prompt
Create spec.md using the approved structure and assumptions.

Keep the document concise, implementation-focused, and aligned with the case study.

Do not introduce additional requirements.

### Outcome
Produced the complete project specification that was reviewed before implementation.

---

## 6. Solution Architecture

### Objective
Design a lightweight solution architecture suitable for the assignment.

### Prompt
Recommend a practical project structure using:
- .NET 8 Minimal API
- Core business library
- xUnit tests
- Angular frontend

Clearly separate API concerns, business logic, provider implementations, pricing, validation, and booking storage.

Do not generate code yet.

### Outcome
Defined the solution architecture including:
- CarRental.Api
- CarRental.Core
- CarRental.Tests
- Angular UI
- Responsibilities for each layer

After scaffolding, the solution was manually validated by running:
- `dotnet restore`
- `dotnet build`
- `dotnet test`

All commands completed successfully before proceeding to implementation.

---

## 7. Project Scaffolding

### Objective
Create the initial solution structure.

### Prompt
Scaffold the .NET solution and projects based on the approved architecture.

Create:
- Solution file
- API project
- Core class library
- Test project
- Required folders
- Project references

Do not implement business logic or endpoints.

### Outcome
Created the initial solution structure and project references, ready for implementation.

---

## 8. Domain Models and Provider Abstraction

### Objective
Create the initial business models and the common contract for rental providers.

### Representative Prompt
Create the core domain models and the provider interface in the CarRental.Core project.

Keep the models independent from the API layer. Use enums where suitable and use simple immutable models. Create only the domain models and ICarRentalProvider in this step. Do not implement providers, pricing, validation, storage, services, or endpoints yet.

Before creating the files, show the planned classes, properties, and interface methods for review.

### Outcome
Created the search, offer, booking, and booking confirmation models along with supporting enums.

Created ICarRentalProvider as the common interface that PremiumDrive and BudgetWheels will implement.

The generated design was reviewed and refined to include vehicle name, insurance type, currency, and provider information. The complete solution build succeeded after the changes.



## 9. Pricing Rules and Unit Tests

### Objective
Implement and verify the provider-specific rental pricing rules.

### Representative Prompt
Review the pricing rules in `spec.md` and suggest a small pricing design before creating files.

Use one shared helper for rental nights. The pickup date should be included and the return date should be excluded.

PremiumDrive should use flat daily pricing.

BudgetWheels should calculate every rental night separately and apply a 20 percent surcharge to Friday, Saturday, and Sunday nights.

Keep the pricing interface simple and expose only the final total price calculation. Add unit tests for weekday, weekend, mixed-date, single-night, and invalid-date cases.

Do not implement providers, services, storage, endpoints, or frontend code in this step.

### Outcome
Created the shared rental-night calculator and separate pricing calculators for PremiumDrive and BudgetWheels.

Added unit tests for date boundaries, flat pricing, weekend surcharges, mixed rental periods, and invalid date ranges.

One incorrect test expectation was identified during execution, reviewed, and corrected. All 13 tests passed successfully.

## 10. Provider Stubs and Offer Mapping

### Objective
Implement deterministic provider integrations that return normalized rental offers using the provider-specific pricing rules.

### Representative Prompt
Pricing logic is completed and the tests are passing.

Please implement the PremiumDrive and BudgetWheels provider stubs using the existing `ICarRentalProvider` contract.

Each provider should receive its matching concrete pricing calculator in the constructor.

Keep the fixed vehicle data separate from `CarOffer` by using a small private record or class inside each provider.

When `SearchAsync` is called:

- apply category filtering only when a category is supplied,
- map the fixed vehicle data into `CarOffer`,
- calculate the total price using the provider pricing calculator,
- populate the provider-specific insurance, cancellation policy, currency, and availability.

PremiumDrive should always return available offers.

BudgetWheels should return both available and unavailable offers. Do not filter unavailable offers inside the provider because that will be handled by the search service later.

Add focused unit tests for mapping, category filtering, provider-specific totals, insurance, cancellation policy, and availability.

Do not implement search orchestration, booking, validation, storage, endpoints, or frontend code yet.

### Outcome
Created deterministic provider stubs for PremiumDrive and BudgetWheels.

Each provider now maps private fixed vehicle data into normalized `CarOffer` results and calculates totals using its matching pricing calculator.

PremiumDrive returns only available offers, while BudgetWheels preserves both available and unavailable offers for later filtering by the search service.

## 11. Search Orchestration

### Objective
Combine rental offers from all providers and return only available offers sorted by total price.

### Representative Prompt
Review `spec.md` and the existing provider contracts.

Implement the search orchestration layer using `ICarSearchService` and `CarSearchService`.

The service should call all registered providers with `Task.WhenAll`, combine the results, remove unavailable offers, sort by `TotalPrice`, and return the final list.

Keep the service provider-agnostic and add focused unit tests using fake providers.

### Outcome
Created a provider-agnostic search service that calls all rental providers in parallel, combines their results, removes unavailable offers, and sorts the remaining offers by total price.

Added unit tests for result aggregation, availability filtering, sorting, empty results, cancellation-token forwarding, and provider exception propagation.

## AI Usage Summary

AI was used to:
- Understand and analyse the requirements.
- Identify ambiguities.
- Refine implementation assumptions.
- Plan the specification.
- Draft project documentation.

All generated content was reviewed, validated, and refined before being accepted.