namespace CarRental.Core.Validation;

public sealed record ValidationResult
{
    public ValidationResult(IReadOnlyList<ValidationIssue> errors)
    {
        Errors = errors;
    }

    public IReadOnlyList<ValidationIssue> Errors { get; }

    public bool IsValid => Errors.Count == 0;

    public static ValidationResult Success() => new(Array.Empty<ValidationIssue>());

    public static ValidationResult Failure(IEnumerable<ValidationIssue> errors) => new(errors.ToArray());
}