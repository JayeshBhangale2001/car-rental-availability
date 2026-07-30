using CarRental.Core.Domain;
using CarRental.Core.Services;
using CarRental.Core.Storage;
using CarRental.Core.Validation;

namespace CarRental.Tests.Unit;

public class BookingServiceTests
{
    [Fact]
    public async Task CreateBookingAsync_ValidBooking_SavesBookingAndReturnsConfirmation()
    {
        var validator = new PassThroughValidator();
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-1001");
        var service = new BookingService(validator, store, referenceGenerator);

        var booking = CreateBooking();

        var result = await service.CreateBookingAsync(booking);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Confirmation);
        Assert.Equal("BK-1001", result.Confirmation.ReferenceNumber);
        Assert.Equal("PremiumDrive", result.Confirmation.ProviderName);
        Assert.Equal(VehicleCategory.Economy, result.Confirmation.Category);
        Assert.Equal(3600m, result.Confirmation.TotalPrice);
        Assert.Equal("Free cancellation up to 48h before pickup", result.Confirmation.CancellationPolicy);

        Assert.NotNull(store.LastSavedBooking);
        Assert.Equal("BK-1001", store.LastSavedBooking.ReferenceNumber);
    }

    [Fact]
    public async Task CreateBookingAsync_InvalidBooking_ReturnsValidationErrorsWithoutSaving()
    {
        var invalidResult = ValidationResult.Failure(new[]
        {
            new ValidationIssue(
                ValidationIssueKind.BusinessRule,
                "documentType",
                "booking.document.mismatch",
                "Domestic pickup requires National ID.")
        });

        var validator = new FixedResultValidator(invalidResult);
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-2001");
        var service = new BookingService(validator, store, referenceGenerator);

        var result = await service.CreateBookingAsync(CreateBooking());

        Assert.False(result.IsSuccess);
        Assert.Null(result.Confirmation);
        Assert.False(result.ValidationResult.IsValid);
        Assert.Contains(result.ValidationResult.Errors, e => e.Code == "booking.document.mismatch");
        Assert.Null(store.LastSavedBooking);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenReferenceAlreadyExists_GeneratesAnotherReference()
    {
        var validator = new PassThroughValidator();
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-DUP", "BK-DUP", "BK-UNIQUE");
        var service = new BookingService(validator, store, referenceGenerator);

        var firstResult = await service.CreateBookingAsync(CreateBooking());
        var secondResult = await service.CreateBookingAsync(CreateBooking());

        Assert.True(firstResult.IsSuccess);
        Assert.True(secondResult.IsSuccess);
        Assert.Equal("BK-DUP", firstResult.Confirmation?.ReferenceNumber);
        Assert.Equal("BK-UNIQUE", secondResult.Confirmation?.ReferenceNumber);
    }

    [Fact]
    public async Task CreateBookingAsync_PassesCancellationTokenToStore()
    {
        var validator = new PassThroughValidator();
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-3001");
        var service = new BookingService(validator, store, referenceGenerator);
        using var cts = new CancellationTokenSource();

        await service.CreateBookingAsync(CreateBooking(), cts.Token);

        Assert.Equal(cts.Token, store.LastCancellationToken);
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsBooking_WhenFound()
    {
        var validator = new PassThroughValidator();
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-4001");
        var service = new BookingService(validator, store, referenceGenerator);

        await service.CreateBookingAsync(CreateBooking());

        var stored = await service.GetBookingByReferenceAsync("BK-4001");

        Assert.NotNull(stored);
        Assert.Equal("BK-4001", stored.ReferenceNumber);
    }

    [Fact]
    public async Task GetBookingByReferenceAsync_ReturnsNull_WhenNotFound()
    {
        var validator = new PassThroughValidator();
        var store = new FakeBookingStore();
        var referenceGenerator = new SequenceReferenceGenerator("BK-5001");
        var service = new BookingService(validator, store, referenceGenerator);

        var stored = await service.GetBookingByReferenceAsync("BK-404");

        Assert.Null(stored);
    }

    private static Booking CreateBooking()
    {
        return new Booking
        {
            ReferenceNumber = "TEMP",
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
            BookedAtUtc = default
        };
    }

    private sealed class PassThroughValidator : IValidator<Booking>
    {
        public ValidationResult Validate(Booking model) => ValidationResult.Success();
    }

    private sealed class FixedResultValidator : IValidator<Booking>
    {
        private readonly ValidationResult result;

        public FixedResultValidator(ValidationResult result)
        {
            this.result = result;
        }

        public ValidationResult Validate(Booking model) => result;
    }

    private sealed class SequenceReferenceGenerator : IBookingReferenceGenerator
    {
        private readonly Queue<string> references;

        public SequenceReferenceGenerator(params string[] references)
        {
            this.references = new Queue<string>(references);
        }

        public string CreateReference()
        {
            return references.Count > 0
                ? references.Dequeue()
                : "BK-FALLBACK";
        }
    }

    private sealed class FakeBookingStore : IBookingStore
    {
        private readonly Dictionary<string, Booking> bookings = new(StringComparer.OrdinalIgnoreCase);

        public Booking? LastSavedBooking { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }

        public Task<bool> TrySaveAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            LastCancellationToken = cancellationToken;

            if (bookings.ContainsKey(booking.ReferenceNumber))
            {
                return Task.FromResult(false);
            }

            bookings[booking.ReferenceNumber] = booking;
            LastSavedBooking = booking;
            return Task.FromResult(true);
        }

        public Task<Booking?> GetByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default)
        {
            LastCancellationToken = cancellationToken;
            bookings.TryGetValue(referenceNumber, out var booking);
            return Task.FromResult(booking);
        }
    }
}