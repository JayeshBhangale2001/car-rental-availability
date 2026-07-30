using CarRental.Core.Domain;
using CarRental.Core.Pricing;
using CarRental.Core.Providers;

namespace CarRental.Tests.Unit;

public class PremiumDriveProviderTests
{
    [Fact]
    public async Task SearchAsync_ReturnsFreeCancellationPolicyForAllOffers()
    {
        var provider = new PremiumDriveProvider(new PremiumDrivePricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Mumbai",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 4)
        });

        Assert.NotEmpty(offers);
        Assert.All(
            offers,
            offer => Assert.Equal("Free cancellation up to 48h before pickup", offer.CancellationPolicy));
    }

    [Fact]
    public async Task SearchAsync_WhenCategoryIsNotSpecified_ReturnsAllAvailableOffers()
    {
        var provider = new PremiumDriveProvider(new PremiumDrivePricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Mumbai",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 4)
        });

        Assert.Equal(4, offers.Count);
        Assert.All(offers, offer => Assert.True(offer.IsAvailable));
        Assert.All(offers, offer => Assert.Equal("PremiumDrive", offer.ProviderName));
    }

    [Fact]
    public async Task SearchAsync_WhenCategoryIsSpecified_ReturnsOnlyMatchingOffers()
    {
        var provider = new PremiumDriveProvider(new PremiumDrivePricingCalculator());

        var offers = await provider.SearchAsync(new SearchCriteria
        {
            PickupLocation = "Mumbai",
            PickupLocationType = PickupLocationType.Domestic,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 4),
            Category = VehicleCategory.SUV
        });

        Assert.Single(offers);
        var offer = offers[0];
        Assert.Equal("PD-SUV-001", offer.OfferId);
        Assert.Equal("PremiumDrive XUV", offer.VehicleName);
        Assert.Equal(VehicleCategory.SUV, offer.Category);
        Assert.Equal(4200m, offer.PerDayRate);
        Assert.Equal(12600m, offer.TotalPrice);
        Assert.Equal("Comprehensive Insurance", offer.InsuranceType);
        Assert.Equal("Free cancellation up to 48h before pickup", offer.CancellationPolicy);
        Assert.Equal("INR", offer.Currency);
        Assert.True(offer.IsAvailable);
    }
}