namespace CarRental.Core.Pricing;

public sealed class BudgetWheelsPricingCalculator : IPricingCalculator
{
    private const decimal WeekendSurchargeMultiplier = 1.20m;

    public string ProviderName => "BudgetWheels";

    public decimal CalculateTotalPrice(
        decimal baseDailyRate,
        DateOnly pickupDate,
        DateOnly returnDate)
    {
        var totalPrice = 0m;

        foreach (var rentalNight in RentalNightCalculator.GetRentalNights(pickupDate, returnDate))
        {
            totalPrice += IsWeekendNight(rentalNight.DayOfWeek)
                ? baseDailyRate * WeekendSurchargeMultiplier
                : baseDailyRate;
        }

        return totalPrice;
    }

    private static bool IsWeekendNight(DayOfWeek dayOfWeek)
    {
        return dayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}