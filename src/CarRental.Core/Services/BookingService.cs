using CarRental.Core.Domain;
using CarRental.Core.Storage;
using CarRental.Core.Validation;

namespace CarRental.Core.Services;

public sealed class BookingService : IBookingService
{
    private const int MaxReferenceGenerationAttempts = 10;

    private readonly IValidator<Booking> bookingValidator;
    private readonly IBookingStore bookingStore;
    private readonly IBookingReferenceGenerator referenceGenerator;

    public BookingService(
        IValidator<Booking> bookingValidator,
        IBookingStore bookingStore,
        IBookingReferenceGenerator referenceGenerator)
    {
        this.bookingValidator = bookingValidator;
        this.bookingStore = bookingStore;
        this.referenceGenerator = referenceGenerator;
    }

    public async Task<BookingCreateResult> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(booking);

        var validationResult = bookingValidator.Validate(booking);
        if (!validationResult.IsValid)
        {
            return new BookingCreateResult
            {
                IsSuccess = false,
                ValidationResult = validationResult
            };
        }

        for (var attempt = 0; attempt < MaxReferenceGenerationAttempts; attempt++)
        {
            var reference = referenceGenerator.CreateReference();
            var bookingToSave = booking with
            {
                ReferenceNumber = reference,
                BookedAtUtc = DateTimeOffset.UtcNow
            };

            var saved = await bookingStore.TrySaveAsync(bookingToSave, cancellationToken);
            if (!saved)
            {
                continue;
            }

            return new BookingCreateResult
            {
                IsSuccess = true,
                Confirmation = new BookingConfirmation
                {
                    ReferenceNumber = bookingToSave.ReferenceNumber,
                    ProviderName = bookingToSave.ProviderName,
                    Category = bookingToSave.SelectedOffer.Category,
                    TotalPrice = bookingToSave.SelectedOffer.TotalPrice,
                    CancellationPolicy = bookingToSave.SelectedOffer.CancellationPolicy
                },
                ValidationResult = ValidationResult.Success()
            };
        }

        throw new InvalidOperationException("Unable to create a unique booking reference.");
    }

    public Task<Booking?> GetBookingByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return bookingStore.GetByReferenceAsync(referenceNumber, cancellationToken);
    }
}