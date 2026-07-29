namespace CarRental.Core.Domain;

public sealed record CarOffer
{
    public required string ProviderName { get; init; }

    public required string OfferId { get; init; }

    public required string VehicleName { get; init; }

    public required VehicleCategory Category { get; init; }

    public required decimal PerDayRate { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string InsuranceType { get; init; }

    public required string Currency { get; init; }

    public required string CancellationPolicy { get; init; }

    public required bool IsAvailable { get; init; }
}