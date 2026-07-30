using CarRental.Core.Domain;
using CarRental.Core.Pricing;
using CarRental.Core.Providers;

namespace CarRental.Tests.Unit;

public class BudgetWheelsProviderTests
{
    [Fact]
    public async Task SearchAsync_ReturnsNonRefundableCancellationPolicyForAllOffers()
    {
        var provider = new BudgetWheelsProvider(new BudgetWheelsPricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Delhi",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 2),
            ReturnDate = new DateOnly(2026, 7, 6)
        });

        Assert.NotEmpty(offers);
        Assert.All(offers, offer => Assert.Equal("Non-refundable", offer.CancellationPolicy));
    }

    [Fact]
    public async Task SearchAsync_WhenCategoryIsNotSpecified_ReturnsAvailableAndUnavailableOffers()
    {
        var provider = new BudgetWheelsProvider(new BudgetWheelsPricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Delhi",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 2),
            ReturnDate = new DateOnly(2026, 7, 6)
        });

        Assert.Equal(4, offers.Count);
        Assert.Contains(offers, offer => offer.OfferId == "BW-COMP-001" && !offer.IsAvailable);
        Assert.Contains(offers, offer => offer.OfferId == "BW-MINI-001" && !offer.IsAvailable);
        Assert.All(offers, offer => Assert.Equal("BudgetWheels", offer.ProviderName));
    }

    [Fact]
    public async Task SearchAsync_WhenCategoryIsSpecified_ReturnsOnlyMatchingBudgetWheelsOffer()
    {
        var provider = new BudgetWheelsProvider(new BudgetWheelsPricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Delhi",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 2),
            ReturnDate = new DateOnly(2026, 7, 6),
            Category = VehicleCategory.Economy
        });

        Assert.Single(offers);
        var offer = offers[0];
        Assert.Equal("BW-ECON-001", offer.OfferId);
        Assert.Equal("BudgetWheels Nano", offer.VehicleName);
        Assert.Equal(VehicleCategory.Economy, offer.Category);
        Assert.Equal(1200m, offer.PerDayRate);
        Assert.Equal(5520m, offer.TotalPrice);
        Assert.Equal("Basic Insurance", offer.InsuranceType);
        Assert.Equal("Non-refundable", offer.CancellationPolicy);
        Assert.Equal("INR", offer.Currency);
        Assert.True(offer.IsAvailable);
    }
}