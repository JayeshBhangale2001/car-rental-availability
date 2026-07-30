namespace CarRental.Core.Validation;

public sealed record ValidationIssue(
    ValidationIssueKind Kind,
    string Field,
    string Code,
    string Message);