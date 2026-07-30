namespace CarRental.Core.Pricing;

public sealed class PremiumDrivePricingCalculator : IPricingCalculator
{
    public string ProviderName => "PremiumDrive";

    public decimal CalculateTotalPrice(
        decimal baseDailyRate,
        DateOnly pickupDate,
        DateOnly returnDate)
    {
        var rentalNightCount = RentalNightCalculator.GetRentalNightCount(pickupDate, returnDate);
        return baseDailyRate * rentalNightCount;
    }
}