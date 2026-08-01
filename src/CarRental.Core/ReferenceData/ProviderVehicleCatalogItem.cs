using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public sealed record ProviderVehicleCatalogItem(
    string OfferId,
    string VehicleName,
    VehicleCategory Category,
    decimal BaseDailyRate,
    bool IsAvailable);
