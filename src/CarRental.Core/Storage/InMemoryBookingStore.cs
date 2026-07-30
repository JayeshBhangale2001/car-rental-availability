using System.Collections.Concurrent;
using CarRental.Core.Domain;

namespace CarRental.Core.Storage;

public sealed class InMemoryBookingStore : IBookingStore
{
    private readonly ConcurrentDictionary<string, Booking> bookings = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> TrySaveAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(booking);
        cancellationToken.ThrowIfCancellationRequested();

        var saved = bookings.TryAdd(booking.ReferenceNumber, booking);
        return Task.FromResult(saved);
    }

    public Task<Booking?> GetByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            return Task.FromResult<Booking?>(null);
        }

        bookings.TryGetValue(referenceNumber.Trim(), out var booking);
        return Task.FromResult(booking);
    }
}