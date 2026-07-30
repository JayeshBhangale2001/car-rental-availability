using CarRental.Core.Pricing;

namespace CarRental.Tests.Unit;

public class PremiumDrivePricingCalculatorTests
{
    [Fact]
    public void CalculateTotalPrice_MultipliesBaseRateByRentalNightCount()
    {
        var calculator = new PremiumDrivePricingCalculator();

        var totalPrice = calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 4));

        Assert.Equal(3000m, totalPrice);
    }

    [Fact]
    public void CalculateTotalPrice_ForSingleNight_ReturnsBaseDailyRate()
    {
        var calculator = new PremiumDrivePricingCalculator();

        var totalPrice = calculator.CalculateTotalPrice(
            1250m,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 2));

        Assert.Equal(1250m, totalPrice);
    }

    [Fact]
    public void CalculateTotalPrice_WhenSameDayRental_ThrowsArgumentException()
    {
        var calculator = new PremiumDrivePricingCalculator();

        Assert.Throws<ArgumentException>(() => calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 1)));
    }

    [Fact]
    public void CalculateTotalPrice_WhenReturnDateBeforePickup_ThrowsArgumentException()
    {
        var calculator = new PremiumDrivePricingCalculator();

        Assert.Throws<ArgumentException>(() => calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 2),
            new DateOnly(2026, 7, 1)));
    }
}