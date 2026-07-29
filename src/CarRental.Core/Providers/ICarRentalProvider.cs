using CarRental.Core.Domain;

namespace CarRental.Core.Providers;

public interface ICarRentalProvider
{
    string ProviderName { get; }

    Task<IReadOnlyList<CarOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
}