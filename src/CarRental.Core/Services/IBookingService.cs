using CarRental.Core.Domain;

namespace CarRental.Core.Services;

public interface IBookingService
{
    Task<BookingCreateResult> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetBookingByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default);
}