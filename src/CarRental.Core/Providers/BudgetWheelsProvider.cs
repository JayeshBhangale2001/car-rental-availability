using CarRental.Core.Domain;
using CarRental.Core.Pricing;
using CarRental.Core.ReferenceData;

namespace CarRental.Core.Providers;

public sealed class BudgetWheelsProvider : ICarRentalProvider
{
    private readonly BudgetWheelsPricingCalculator pricingCalculator;
    private readonly BudgetWheelsCatalog catalog;

    public BudgetWheelsProvider(
        BudgetWheelsPricingCalculator pricingCalculator,
        BudgetWheelsCatalog catalog)
    {
        this.pricingCalculator = pricingCalculator;
        this.catalog = catalog;
    }

    public string ProviderName => catalog.ProviderDisplayName;

    public Task<IReadOnlyList<CarOffer>> SearchAsync(
        SearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var offers = catalog.Vehicles
            .Where(vehicle => catalog.ExposedCategories.Contains(vehicle.Category))
            .Where(vehicle => criteria.Category is null || vehicle.Category == criteria.Category)
            .Select(vehicle => MapToOffer(vehicle, criteria))
            .ToArray();

        return Task.FromResult<IReadOnlyList<CarOffer>>(offers);
    }

    private CarOffer MapToOffer(ProviderVehicleCatalogItem vehicle, SearchCriteria criteria)
    {
        return new CarOffer
        {
            ProviderName = catalog.ProviderDisplayName,
            OfferId = vehicle.OfferId,
            VehicleName = vehicle.VehicleName,
            Category = vehicle.Category,
            PerDayRate = vehicle.BaseDailyRate,
            TotalPrice = pricingCalculator.CalculateTotalPrice(
                vehicle.BaseDailyRate,
                criteria.PickupDate,
                criteria.ReturnDate),
            InsuranceType = catalog.InsuranceName,
            Currency = catalog.Currency,
            CancellationPolicy = catalog.CancellationPolicy,
            IsAvailable = vehicle.IsAvailable
        };
    }
}