using CarRental.Core.Domain;

namespace CarRental.Core.Services;

public interface ICarSearchService
{
    Task<IReadOnlyList<CarOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default);
}