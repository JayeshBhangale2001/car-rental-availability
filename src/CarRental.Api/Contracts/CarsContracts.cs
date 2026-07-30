namespace CarRental.Api.Contracts;

public sealed record SearchCarsRequestDto
{
    public string? Pickup { get; init; }

    public string? From { get; init; }

    public string? To { get; init; }

    public string? Category { get; init; }
}

public sealed record SearchCarResponseDto
{
    public required string Provider { get; init; }

    public required string OfferId { get; init; }

    public required string VehicleName { get; init; }

    public required string Category { get; init; }

    public required decimal PerDayRate { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string CancellationPolicy { get; init; }

    public required bool InsuranceIncluded { get; init; }

    public required string Currency { get; init; }
}

public sealed record BookCarRequestDto
{
    public string? Provider { get; init; }

    public string? OfferId { get; init; }

    public string? DriverName { get; init; }

    public string? DocumentType { get; init; }

    public string? DocumentNumber { get; init; }

    public string? Pickup { get; init; }

    public string? From { get; init; }

    public string? To { get; init; }
}

public sealed record BookingConfirmationResponseDto
{
    public required string Reference { get; init; }

    public required string Provider { get; init; }

    public required string Category { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string CancellationPolicy { get; init; }
}

public sealed record BookingOfferResponseDto
{
    public required string Provider { get; init; }

    public required string OfferId { get; init; }

    public required string VehicleName { get; init; }

    public required string Category { get; init; }

    public required decimal PerDayRate { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string CancellationPolicy { get; init; }

    public required string InsuranceType { get; init; }

    public required bool InsuranceIncluded { get; init; }

    public required string Currency { get; init; }
}

public sealed record BookingDetailsResponseDto
{
    public required string Reference { get; init; }

    public required string Provider { get; init; }

    public required string DriverName { get; init; }

    public required string DocumentType { get; init; }

    public required string DocumentNumber { get; init; }

    public required string Pickup { get; init; }

    public required string PickupLocationType { get; init; }

    public required DateOnly From { get; init; }

    public required DateOnly To { get; init; }

    public required BookingOfferResponseDto Offer { get; init; }

    public required DateTimeOffset BookedAtUtc { get; init; }
}

public sealed record ApiValidationIssueDto(
    string Kind,
    string Field,
    string Code,
    string Message);

public sealed record ApiValidationErrorResponseDto(
    IReadOnlyList<ApiValidationIssueDto> Errors);

public sealed record BookingNotFoundResponseDto(
    string Message);