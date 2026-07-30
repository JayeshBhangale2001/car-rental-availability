using CarRental.Core.Pricing;

namespace CarRental.Tests.Unit;

public class RentalNightCalculatorTests
{
    [Fact]
    public void GetRentalNights_IncludesPickupAndExcludesReturnDate()
    {
        var rentalNights = RentalNightCalculator.GetRentalNights(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 4));

        Assert.Equal(new[]
        {
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 2),
            new DateOnly(2026, 7, 3)
        }, rentalNights);
    }

    [Fact]
    public void GetRentalNightCount_UsesRentalNights()
    {
        var rentalNightCount = RentalNightCalculator.GetRentalNightCount(
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 7, 4));

        Assert.Equal(3, rentalNightCount);
    }

    [Fact]
    public void GetRentalNights_WhenSameDayRental_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            RentalNightCalculator.GetRentalNights(
                new DateOnly(2026, 7, 1),
                new DateOnly(2026, 7, 1)));

        Assert.Equal("returnDate", exception.ParamName);
    }

    [Fact]
    public void GetRentalNights_WhenReturnDateBeforePickup_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            RentalNightCalculator.GetRentalNights(
                new DateOnly(2026, 7, 2),
                new DateOnly(2026, 7, 1)));

        Assert.Equal("returnDate", exception.ParamName);
    }
}