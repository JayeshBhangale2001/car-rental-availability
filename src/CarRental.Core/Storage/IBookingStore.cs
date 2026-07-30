using CarRental.Core.Domain;

namespace CarRental.Core.Storage;

public interface IBookingStore
{
    Task<bool> TrySaveAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default);
}