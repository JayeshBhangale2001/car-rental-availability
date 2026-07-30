using CarRental.Core.Domain;
using CarRental.Core.Providers;
using CarRental.Core.Services;

namespace CarRental.Tests.Unit;

public class CarSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_CombinesResultsFromAllProviders()
    {
        var providers = new ICarRentalProvider[]
        {
            new FakeProvider("ProviderA", new[]
            {
                CreateOffer("A-1", 3000m, true),
                CreateOffer("A-2", 1800m, true)
            }),
            new FakeProvider("ProviderB", new[]
            {
                CreateOffer("B-1", 2400m, true)
            })
        };

        var service = new CarSearchService(providers);

        var results = await service.SearchAsync(CreateCriteria());

        Assert.Equal(3, results.Count);
        Assert.Contains(results, offer => offer.OfferId == "A-1");
        Assert.Contains(results, offer => offer.OfferId == "A-2");
        Assert.Contains(results, offer => offer.OfferId == "B-1");
    }

    [Fact]
    public async Task SearchAsync_RemovesUnavailableOffers()
    {
        var providers = new ICarRentalProvider[]
        {
            new FakeProvider("ProviderA", new[]
            {
                CreateOffer("A-1", 3000m, true),
                CreateOffer("A-2", 1800m, false)
            }),
            new FakeProvider("ProviderB", new[]
            {
                CreateOffer("B-1", 2200m, false),
                CreateOffer("B-2", 2000m, true)
            })
        };

        var service = new CarSearchService(providers);

        var results = await service.SearchAsync(CreateCriteria());

        Assert.Equal(2, results.Count);
        Assert.DoesNotContain(results, offer => !offer.IsAvailable);
    }

    [Fact]
    public async Task SearchAsync_SortsOffersByTotalPriceAscending()
    {
        var providers = new ICarRentalProvider[]
        {
            new FakeProvider("ProviderA", new[]
            {
                CreateOffer("A-1", 5000m, true),
                CreateOffer("A-2", 1500m, true)
            }),
            new FakeProvider("ProviderB", new[]
            {
                CreateOffer("B-1", 3200m, true)
            })
        };

        var service = new CarSearchService(providers);

        var results = await service.SearchAsync(CreateCriteria());

        Assert.Equal(new[] { "A-2", "B-1", "A-1" }, results.Select(offer => offer.OfferId));
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoProviderHasAvailableOffers()
    {
        var providers = new ICarRentalProvider[]
        {
            new FakeProvider("ProviderA", new[] { CreateOffer("A-1", 2100m, false) }),
            new FakeProvider("ProviderB", new[] { CreateOffer("B-1", 3100m, false) })
        };

        var service = new CarSearchService(providers);

        var results = await service.SearchAsync(CreateCriteria());

        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchAsync_PassesCancellationTokenToAllProviders()
    {
        var providerA = new FakeProvider("ProviderA", new[] { CreateOffer("A-1", 1000m, true) });
        var providerB = new FakeProvider("ProviderB", new[] { CreateOffer("B-1", 2000m, true) });

        var service = new CarSearchService(new ICarRentalProvider[] { providerA, providerB });
        using var cts = new CancellationTokenSource();

        await service.SearchAsync(CreateCriteria(), cts.Token);

        Assert.Equal(cts.Token, providerA.ReceivedCancellationToken);
        Assert.Equal(cts.Token, providerB.ReceivedCancellationToken);
    }

    [Fact]
    public async Task SearchAsync_DoesNotIgnoreProviderExceptions()
    {
        var providers = new ICarRentalProvider[]
        {
            new FakeProvider("ProviderA", new[] { CreateOffer("A-1", 1000m, true) }),
            new ThrowingProvider("ProviderB", new InvalidOperationException("ProviderB failed."))
        };

        var service = new CarSearchService(providers);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SearchAsync(CreateCriteria()));

        Assert.Equal("ProviderB failed.", exception.Message);
    }

    private static SearchCriteria CreateCriteria()
    {
        return new SearchCriteria
        {
            PickupLocation = "Mumbai",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 4)
        };
    }

    private static CarOffer CreateOffer(string offerId, decimal totalPrice, bool isAvailable)
    {
        return new CarOffer
        {
            ProviderName = "Fake",
            OfferId = offerId,
            VehicleName = $"Vehicle-{offerId}",
            Category = VehicleCategory.Compact,
            PerDayRate = 1000m,
            TotalPrice = totalPrice,
            InsuranceType = "Basic Insurance",
            Currency = "INR",
            CancellationPolicy = "Non-refundable",
            IsAvailable = isAvailable
        };
    }

    private sealed class FakeProvider : ICarRentalProvider
    {
        private readonly IReadOnlyList<CarOffer> offers;

        public FakeProvider(string providerName, IReadOnlyList<CarOffer> offers)
        {
            ProviderName = providerName;
            this.offers = offers;
        }

        public string ProviderName { get; }

        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<IReadOnlyList<CarOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            ReceivedCancellationToken = cancellationToken;
            return Task.FromResult(offers);
        }
    }

    private sealed class ThrowingProvider : ICarRentalProvider
    {
        private readonly Exception exception;

        public ThrowingProvider(string providerName, Exception exception)
        {
            ProviderName = providerName;
            this.exception = exception;
        }

        public string ProviderName { get; }

        public Task<IReadOnlyList<CarOffer>> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            throw exception;
        }
    }
}