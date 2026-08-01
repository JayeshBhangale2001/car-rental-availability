using CarRental.Core.Domain;
using CarRental.Core.ReferenceData;
using CarRental.Core.Validation;

namespace CarRental.Tests.Unit;

public class BookingValidatorTests
{
    private readonly BookingValidator validator = new(
        new InMemoryPickupLocationCatalog(),
        new InMemoryDocumentTypeRuleCatalog());

    [Fact]
    public void Validate_ValidDomesticBookingWithNationalId_ReturnsSuccess()
    {
        var booking = CreateBooking("Mumbai", PickupLocationType.Domestic, DocumentType.NationalId);

        var result = validator.Validate(booking);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ValidInternationalBookingWithPassport_ReturnsSuccess()
    {
        var booking = CreateBooking("Dubai", PickupLocationType.International, DocumentType.Passport);

        var result = validator.Validate(booking);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_DomesticBookingWithPassport_ReturnsBusinessRuleError()
    {
        var booking = CreateBooking("Mumbai", PickupLocationType.Domestic, DocumentType.Passport);

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Code == "booking.document.mismatch" &&
            e.Kind == ValidationIssueKind.BusinessRule);
    }

    [Fact]
    public void Validate_InternationalBookingWithNationalId_ReturnsBusinessRuleError()
    {
        var booking = CreateBooking("London", PickupLocationType.International, DocumentType.NationalId);

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Code == "booking.document.mismatch" &&
            e.Kind == ValidationIssueKind.BusinessRule);
    }

    [Fact]
    public void Validate_EmptyDriverName_ReturnsInputError()
    {
        var booking = CreateBooking("Mumbai", PickupLocationType.Domestic, DocumentType.NationalId) with
        {
            DriverName = ""
        };

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "booking.driverName.required");
    }

    [Fact]
    public void Validate_EmptyDocumentNumber_ReturnsInputError()
    {
        var booking = CreateBooking("Mumbai", PickupLocationType.Domestic, DocumentType.NationalId) with
        {
            DocumentNumber = "  "
        };

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "booking.documentNumber.required");
    }

    [Fact]
    public void Validate_UnsupportedPickupLocation_ReturnsInputError()
    {
        var booking = CreateBooking("Pune", PickupLocationType.Domestic, DocumentType.NationalId);

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "booking.pickup.unsupported");
    }

    [Fact]
    public void Validate_InvalidDateRange_ReturnsInputError()
    {
        var booking = CreateBooking("Mumbai", PickupLocationType.Domestic, DocumentType.NationalId) with
        {
            PickupDate = new DateOnly(2026, 7, 10),
            ReturnDate = new DateOnly(2026, 7, 10)
        };

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "booking.dates.invalidRange");
    }

    [Fact]
    public void Validate_PickupTypeMismatch_ReturnsInputError()
    {
        var booking = CreateBooking("Singapore", PickupLocationType.Domestic, DocumentType.NationalId);

        var result = validator.Validate(booking);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "booking.pickup.typeMismatch");
    }

    private static Booking CreateBooking(
        string pickupLocation,
        PickupLocationType pickupLocationType,
        DocumentType documentType)
    {
        return new Booking
        {
            ReferenceNumber = "BK-001",
            ProviderName = "PremiumDrive",
            DriverName = "Jayesh",
            DocumentType = documentType,
            DocumentNumber = "DOC-12345",
            PickupLocation = pickupLocation,
            PickupLocationType = pickupLocationType,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 3),
            SelectedOffer = new CarOffer
            {
                ProviderName = "PremiumDrive",
                OfferId = "PD-001",
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