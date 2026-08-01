using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public sealed class BudgetWheelsCatalog
{
    public string ProviderDisplayName { get; } = "BudgetWheels";

    public string InsuranceName { get; } = "Basic Insurance";

    public string CancellationPolicy { get; } = "Non-refundable";

    public string Currency { get; } = "INR";

    public IReadOnlySet<VehicleCategory> ExposedCategories { get; } = new HashSet<VehicleCategory>
    {
        VehicleCategory.Economy,
        VehicleCategory.Compact,
        VehicleCategory.SUV,
        VehicleCategory.Minivan
    };

    public IReadOnlyList<ProviderVehicleCatalogItem> Vehicles { get; } =
    [
        new ProviderVehicleCatalogItem("BW-ECON-001", "BudgetWheels Nano", VehicleCategory.Economy, 1200m, true),
        new ProviderVehicleCatalogItem("BW-COMP-001", "BudgetWheels Swift", VehicleCategory.Compact, 1600m, false),
        new ProviderVehicleCatalogItem("BW-SUV-001", "BudgetWheels Duster", VehicleCategory.SUV, 2600m, true),
        new ProviderVehicleCatalogItem("BW-MINI-001", "BudgetWheels Ertiga", VehicleCategory.Minivan, 3000m, false)
    ];
}
