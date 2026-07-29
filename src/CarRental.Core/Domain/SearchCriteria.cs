namespace CarRental.Core.Domain;

public sealed record SearchCriteria
{
    public required string PickupLocation { get; init; }

    public required PickupLocationType PickupLocationType { get; init; }

    public required DateOnly PickupDate { get; init; }

    public required DateOnly ReturnDate { get; init; }

    public VehicleCategory? Category { get; init; }
}