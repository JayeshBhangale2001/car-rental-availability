namespace CarRental.Core.Storage;

public sealed class GuidBookingReferenceGenerator : IBookingReferenceGenerator
{
    public string CreateReference()
    {
        return $"BK-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}";
    }
}