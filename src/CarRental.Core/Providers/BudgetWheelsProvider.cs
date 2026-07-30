using CarRental.Core.Domain;
using CarRental.Core.Pricing;

namespace CarRental.Core.Providers;

public sealed class BudgetWheelsProvider : ICarRentalProvider
{
    private const string ProviderNameValue = "BudgetWheels";
    private const string InsuranceTypeValue = "Basic Insurance";
    private const string CancellationPolicyValue = "Non-refundable";
    private const string CurrencyValue = "INR";

    private static readonly IReadOnlyList<BudgetWheelsVehicle> Vehicles = new[]
    {
        new BudgetWheelsVehicle("BW-ECON-001", "BudgetWheels Nano", VehicleCategory.Economy, 1200m, true),
        new BudgetWheelsVehicle("BW-COMP-001", "BudgetWheels Swift", VehicleCategory.Compact, 1600m, false),
        new BudgetWheelsVehicle("BW-SUV-001", "BudgetWheels Duster", VehicleCategory.SUV, 2600m, true),
        new BudgetWheelsVehicle("BW-MINI-001", "BudgetWheels Ertiga", VehicleCategory.Minivan, 3000m, false)
    };

    private readonly BudgetWheelsPricingCalculator pricingCalculator;

    public BudgetWheelsProvider(BudgetWheelsPricingCalculator pricingCalculator)
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

    private CarOffer MapToOffer(BudgetWheelsVehicle vehicle, SearchCriteria criteria)
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
            IsAvailable = vehicle.IsAvailable
        };
    }

    private sealed record BudgetWheelsVehicle(
        string OfferId,
        string VehicleName,
        VehicleCategory Category,
        decimal BaseDailyRate,
        bool IsAvailable);
}