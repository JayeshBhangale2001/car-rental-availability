using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public sealed class InMemoryPickupLocationCatalog : IPickupLocationCatalog
{
    private static readonly string[] DomesticLocationNames =
    {
        "Mumbai",
        "Delhi"
    };

    private static readonly string[] InternationalLocationNames =
    {
        "Dubai",
        "London",
        "Singapore"
    };

    private static readonly HashSet<string> DomesticLocations = new(DomesticLocationNames, StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<string> InternationalLocations = new(InternationalLocationNames, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> GetDomesticLocations() => DomesticLocationNames;

    public IReadOnlyList<string> GetInternationalLocations() => InternationalLocationNames;

    public bool TryGetLocationType(string location, out PickupLocationType locationType)
    {
        var normalized = Normalize(location);
        if (normalized is null)
        {
            locationType = default;
            return false;
        }

        if (DomesticLocations.Contains(normalized))
        {
            locationType = PickupLocationType.Domestic;
            return true;
        }

        if (InternationalLocations.Contains(normalized))
        {
            locationType = PickupLocationType.International;
            return true;
        }

        locationType = default;
        return false;
    }

    private static string? Normalize(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return null;
        }

        return location.Trim();
    }
}
