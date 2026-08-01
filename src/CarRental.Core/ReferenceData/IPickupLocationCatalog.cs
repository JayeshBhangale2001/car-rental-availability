using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public interface IPickupLocationCatalog
{
    IReadOnlyList<string> GetDomesticLocations();

    IReadOnlyList<string> GetInternationalLocations();

    bool TryGetLocationType(string location, out PickupLocationType locationType);
}
