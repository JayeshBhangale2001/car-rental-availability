namespace CarRental.Core.Domain;

public sealed record BookingConfirmation
{
    public required string ReferenceNumber { get; init; }

    public required string ProviderName { get; init; }

    public required VehicleCategory Category { get; init; }

    public required decimal TotalPrice { get; init; }

    public required string CancellationPolicy { get; init; }
}