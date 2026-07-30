namespace CarRental.Core.Pricing;

public static class RentalNightCalculator
{
    public static IReadOnlyList<DateOnly> GetRentalNights(DateOnly pickupDate, DateOnly returnDate)
    {
        if (returnDate <= pickupDate)
        {
            throw new ArgumentException("Return date must be after pickup date.", nameof(returnDate));
        }

        var rentalNights = new List<DateOnly>();

        for (var currentDate = pickupDate; currentDate < returnDate; currentDate = currentDate.AddDays(1))
        {
            rentalNights.Add(currentDate);
        }

        return rentalNights;
    }

    public static int GetRentalNightCount(DateOnly pickupDate, DateOnly returnDate)
    {
        return GetRentalNights(pickupDate, returnDate).Count;
    }
}