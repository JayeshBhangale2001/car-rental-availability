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
