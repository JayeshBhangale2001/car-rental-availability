using CarRental.Core.Domain;
using CarRental.Core.Validation;

namespace CarRental.Core.Services;

public sealed record BookingCreateResult
{
    public required bool IsSuccess { get; init; }

    public BookingConfirmation? Confirmation { get; init; }

    public ValidationResult ValidationResult { get; init; } = ValidationResult.Success();
}