using CarRental.Core.Domain;
using CarRental.Core.Storage;

namespace CarRental.Tests.Unit;

public class InMemoryBookingStoreTests
{
    [Fact]
    public async Task TrySaveAsync_SavesBooking_WhenReferenceIsNew()
    {
        var store = new InMemoryBookingStore();
        var booking = CreateBooking("BK-001");

        var saved = await store.TrySaveAsync(booking);
        var stored = await store.GetByReferenceAsync("BK-001");

        Assert.True(saved);
        Assert.NotNull(stored);
        Assert.Equal("BK-001", stored.ReferenceNumber);
    }

    [Fact]
    public async Task TrySaveAsync_DoesNotOverwrite_WhenReferenceAlreadyExists()
    {
        var store = new InMemoryBookingStore();
        var original = CreateBooking("BK-001") with { DriverName = "First" };
        var duplicate = CreateBooking("BK-001") with { DriverName = "Second" };

        var firstSave = await store.TrySaveAsync(original);
        var secondSave = await store.TrySaveAsync(duplicate);
        var stored = await store.GetByReferenceAsync("BK-001");

        Assert.True(firstSave);
        Assert.False(secondSave);
        Assert.NotNull(stored);
        Assert.Equal("First", stored.DriverName);
    }

    [Fact]
    public async Task GetByReferenceAsync_ReturnsNull_WhenReferenceDoesNotExist()
    {
        var store = new InMemoryBookingStore();

        var stored = await store.GetByReferenceAsync("BK-404");

        Assert.Null(stored);
    }

    private static Booking CreateBooking(string referenceNumber)
    {
        return new Booking
        {
            ReferenceNumber = referenceNumber,
            ProviderName = "PremiumDrive",
            DriverName = "Driver",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "DOC-1",
            PickupLocation = "Mumbai",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 3),
            SelectedOffer = new CarOffer
            {
                ProviderName = "PremiumDrive",
                OfferId = "PD-1",
                VehicleName = "PremiumDrive Spark",
                Category = VehicleCategory.Economy,
                PerDayRate = 1800m,
                TotalPrice = 3600m,
                InsuranceType = "Comprehensive Insurance",
                Currency = "INR",
                CancellationPolicy = "Free cancellation up to 48h before pickup",
                IsAvailable = true
            },
            BookedAtUtc = DateTimeOffset.UtcNow
        };
    }
}