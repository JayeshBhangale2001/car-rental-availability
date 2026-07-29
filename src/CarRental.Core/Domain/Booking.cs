namespace CarRental.Core.Domain;

public sealed record Booking
{
    public required string ReferenceNumber { get; init; }

    public required string ProviderName { get; init; }

    public required string DriverName { get; init; }

    public required DocumentType DocumentType { get; init; }

    public required string DocumentNumber { get; init; }

    public required string PickupLocation { get; init; }

    public required PickupLocationType PickupLocationType { get; init; }

    public required DateOnly PickupDate { get; init; }

    public required DateOnly ReturnDate { get; init; }

    public required CarOffer SelectedOffer { get; init; }

    public required DateTimeOffset BookedAtUtc { get; init; }
}