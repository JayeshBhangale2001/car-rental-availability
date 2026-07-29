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

## AI Usage Summary

AI was used to:
- Understand and analyse the requirements.
- Identify ambiguities.
- Refine implementation assumptions.
- Plan the specification.
- Draft project documentation.

All generated content was reviewed, validated, and refined before being accepted.