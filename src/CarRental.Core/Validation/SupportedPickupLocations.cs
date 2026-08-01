using CarRental.Core.Domain;

namespace CarRental.Core.Validation;

public static class SupportedPickupLocations
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

    public static IReadOnlyList<string> GetDomesticLocations() => DomesticLocationNames;

    public static IReadOnlyList<string> GetInternationalLocations() => InternationalLocationNames;

    public static bool IsSupported(string location)
    {
        var normalized = Normalize(location);
        if (normalized is null)
        {
            return false;
        }

        return DomesticLocations.Contains(normalized) || InternationalLocations.Contains(normalized);
    }

    public static bool TryGetLocationType(string location, out PickupLocationType locationType)
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