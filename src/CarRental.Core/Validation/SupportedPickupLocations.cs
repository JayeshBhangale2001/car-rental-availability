using CarRental.Core.Domain;

namespace CarRental.Core.Validation;

public static class SupportedPickupLocations
{
    private static readonly HashSet<string> DomesticLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Mumbai",
        "Delhi"
    };

    private static readonly HashSet<string> InternationalLocations = new(StringComparer.OrdinalIgnoreCase)
    {
        "Dubai",
        "London",
        "Singapore"
    };

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