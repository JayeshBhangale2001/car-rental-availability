using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public sealed class PremiumDriveCatalog
{
    public string ProviderDisplayName { get; } = "PremiumDrive";

    public string InsuranceName { get; } = "Comprehensive Insurance";

    public string CancellationPolicy { get; } = "Free cancellation up to 48h before pickup";

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
        new ProviderVehicleCatalogItem("PD-ECON-001", "PremiumDrive Spark", VehicleCategory.Economy, 1800m, true),
        new ProviderVehicleCatalogItem("PD-COMP-001", "PremiumDrive Accent", VehicleCategory.Compact, 2400m, true),
        new ProviderVehicleCatalogItem("PD-SUV-001", "PremiumDrive XUV", VehicleCategory.SUV, 4200m, true),
        new ProviderVehicleCatalogItem("PD-MINI-001", "PremiumDrive Voyager", VehicleCategory.Minivan, 5100m, true)
    ];
}
