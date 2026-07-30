namespace CarRental.Core.Pricing;

public interface IPricingCalculator
{
    string ProviderName { get; }

    decimal CalculateTotalPrice(
        decimal baseDailyRate,
        DateOnly pickupDate,
        DateOnly returnDate);
}