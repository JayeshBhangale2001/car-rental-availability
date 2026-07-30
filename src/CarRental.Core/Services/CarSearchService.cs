using CarRental.Core.Domain;
using CarRental.Core.Providers;

namespace CarRental.Core.Services;

public sealed class CarSearchService : ICarSearchService
{
    private readonly IReadOnlyList<ICarRentalProvider> providers;

    public CarSearchService(IEnumerable<ICarRentalProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);
        this.providers = providers.ToArray();
    }

    public async Task<IReadOnlyList<CarOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var providerTasks = providers
            .Select(provider => provider.SearchAsync(criteria, cancellationToken));

        var providerResults = await Task.WhenAll(providerTasks);

        return providerResults
            .SelectMany(offers => offers)
            .Where(offer => offer.IsAvailable)
            .OrderBy(offer => offer.TotalPrice)
            .ToArray();
    }
}