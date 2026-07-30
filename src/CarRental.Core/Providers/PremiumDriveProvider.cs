using CarRental.Core.Domain;
using CarRental.Core.Pricing;

namespace CarRental.Core.Providers;

public sealed class PremiumDriveProvider : ICarRentalProvider
{
    private const string ProviderNameValue = "PremiumDrive";
    private const string InsuranceTypeValue = "Comprehensive Insurance";
    private const string CancellationPolicyValue = "Free cancellation up to 48h before pickup";
    private const string CurrencyValue = "INR";

    private static readonly IReadOnlyList<PremiumDriveVehicle> Vehicles = new[]
    {
        new PremiumDriveVehicle("PD-ECON-001", "PremiumDrive Spark", VehicleCategory.Economy, 1800m),
        new PremiumDriveVehicle("PD-COMP-001", "PremiumDrive Accent", VehicleCategory.Compact, 2400m),
        new PremiumDriveVehicle("PD-SUV-001", "PremiumDrive XUV", VehicleCategory.SUV, 4200m),
        new PremiumDriveVehicle("PD-MINI-001", "PremiumDrive Voyager", VehicleCategory.Minivan, 5100m)
    };

    private readonly PremiumDrivePricingCalculator pricingCalculator;

    public PremiumDriveProvider(PremiumDrivePricingCalculator pricingCalculator)
    {
        this.pricingCalculator = pricingCalculator;
    }

    public string ProviderName => ProviderNameValue;

    public Task<IReadOnlyList<CarOffer>> SearchAsync(
        SearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var offers = Vehicles
            .Where(vehicle => criteria.Category is null || vehicle.Category == criteria.Category)
            .Select(vehicle => MapToOffer(vehicle, criteria))
            .ToArray();

        return Task.FromResult<IReadOnlyList<CarOffer>>(offers);
    }

    private CarOffer MapToOffer(PremiumDriveVehicle vehicle, SearchCriteria criteria)
    {
        return new CarOffer
        {
            ProviderName = ProviderNameValue,
            OfferId = vehicle.OfferId,
            VehicleName = vehicle.VehicleName,
            Category = vehicle.Category,
            PerDayRate = vehicle.BaseDailyRate,
            TotalPrice = pricingCalculator.CalculateTotalPrice(
                vehicle.BaseDailyRate,
                criteria.PickupDate,
                criteria.ReturnDate),
            InsuranceType = InsuranceTypeValue,
            Currency = CurrencyValue,
            CancellationPolicy = CancellationPolicyValue,
            IsAvailable = true
        };
    }

    private sealed record PremiumDriveVehicle(
        string OfferId,
        string VehicleName,
        VehicleCategory Category,
        decimal BaseDailyRate);
}