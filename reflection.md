# Reflection

This project moved smoothly once the Core layer boundaries were clear: providers, pricing, validation, and booking service logic were already separated well, so the API layer could stay thin.

What worked well with Copilot:
- Rapidly scaffolding Minimal API endpoints from the spec.
- Mapping request/response DTOs consistently across search, booking, and booking lookup.
- Speeding up integration test setup with WebApplicationFactory, including happy-path and error-case coverage.
- Adding OpenAPI metadata (endpoint names, response types, status codes) with minimal friction.

What needed manual review:
- Status code semantics, especially ensuring business-rule validation returns 422 and input validation returns 400.
- Date parsing strictness (YYYY-MM-DD) and enum parsing behavior.
- Verifying booking flow validates search criteria before provider calls, so invalid requests do not trigger provider searches.
- Sanity-checking sample requests and documentation details for reviewer clarity.

Key takeaways:
- Keeping all business rules in Core made API handlers easier to reason about and test.
- Integration tests were essential to confirm end-to-end contract behavior, not just unit-level logic.
- Explicit OpenAPI metadata improves maintainability and makes review/demo easier.
- Small iterative checks after each change prevented regressions and kept the solution aligned with the specification.

Frontend reflection:
- Copilot helped scaffold the Angular workspace quickly, generate standalone feature components, and wire the root flow for search, booking, confirmation, and booking lookup with minimal boilerplate.
- Manual review was still required for strict TypeScript initialization details, form validation behavior, and clear UI messaging for API validation errors and not-found responses.
- Integrating the UI with the API reinforced the value of stable DTO contracts: keeping frontend models aligned with backend responses made service integration straightforward and reduced mapping bugs.
