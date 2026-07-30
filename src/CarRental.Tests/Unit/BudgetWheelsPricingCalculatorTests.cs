using CarRental.Core.Pricing;

namespace CarRental.Tests.Unit;

public class BudgetWheelsPricingCalculatorTests
{
    [Fact]
    public void CalculateTotalPrice_AppliesWeekendSurchargeToFridaySaturdayAndSundayNights()
    {
        var calculator = new BudgetWheelsPricingCalculator();

        var totalPrice = calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 2),
            new DateOnly(2026, 7, 6));

        Assert.Equal(4600m, totalPrice);
    }

    [Fact]
    public void CalculateTotalPrice_UsesBaseRateForWeekdayNights()
    {
        var calculator = new BudgetWheelsPricingCalculator();

        var totalPrice = calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 6),
            new DateOnly(2026, 7, 8));

        Assert.Equal(2000m, totalPrice);
    }

    [Fact]
    public void CalculateTotalPrice_ForSingleFridayNight_AppliesSurcharge()
    {
        var calculator = new BudgetWheelsPricingCalculator();

        var totalPrice = calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 3),
            new DateOnly(2026, 7, 4));

        Assert.Equal(1200m, totalPrice);
    }

    [Fact]
    public void CalculateTotalPrice_WhenSameDayRental_ThrowsArgumentException()
    {
        var calculator = new BudgetWheelsPricingCalculator();

        Assert.Throws<ArgumentException>(() => calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 1)));
    }

    [Fact]
    public void CalculateTotalPrice_WhenReturnDateBeforePickup_ThrowsArgumentException()
    {
        var calculator = new BudgetWheelsPricingCalculator();

        Assert.Throws<ArgumentException>(() => calculator.CalculateTotalPrice(
            1000m,
            new DateOnly(2026, 7, 2),
            new DateOnly(2026, 7, 1)));
    }
}