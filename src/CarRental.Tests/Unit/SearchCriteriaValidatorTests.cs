using CarRental.Core.Domain;
using CarRental.Core.ReferenceData;
using CarRental.Core.Validation;

namespace CarRental.Tests.Unit;

public class SearchCriteriaValidatorTests
{
    private readonly SearchCriteriaValidator validator = new(new InMemoryPickupLocationCatalog());

    [Fact]
    public void Validate_ValidDomesticCriteria_ReturnsSuccess()
    {
        var result = validator.Validate(CreateCriteria("Mumbai", PickupLocationType.Domestic));

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ValidInternationalCriteriaWithCaseInsensitiveLocation_ReturnsSuccess()
    {
        var result = validator.Validate(CreateCriteria("  loNDoN  ", PickupLocationType.International));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyPickupLocation_ReturnsInputError()
    {
        var result = validator.Validate(CreateCriteria("   ", PickupLocationType.Domestic));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "search.pickup.required" && e.Kind == ValidationIssueKind.Input);
    }

    [Fact]
    public void Validate_UnsupportedPickupLocation_ReturnsInputError()
    {
        var result = validator.Validate(CreateCriteria("Pune", PickupLocationType.Domestic));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "search.pickup.unsupported");
    }

    [Fact]
    public void Validate_InvalidDateRange_ReturnsInputError()
    {
        var result = validator.Validate(CreateCriteria("Mumbai", PickupLocationType.Domestic) with
        {
            PickupDate = new DateOnly(2026, 7, 10),
            ReturnDate = new DateOnly(2026, 7, 10)
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "search.dates.invalidRange");
    }

    [Fact]
    public void Validate_InvalidCategoryEnumValue_ReturnsInputError()
    {
        var result = validator.Validate(CreateCriteria("Mumbai", PickupLocationType.Domestic) with
        {
            Category = (VehicleCategory)999
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "search.category.invalid");
    }

    [Fact]
    public void Validate_PickupTypeMismatch_ReturnsInputError()
    {
        var result = validator.Validate(CreateCriteria("Singapore", PickupLocationType.Domestic));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == "search.pickup.typeMismatch");
    }

    private static SearchCriteria CreateCriteria(string pickupLocation, PickupLocationType pickupLocationType)
    {
        return new SearchCriteria
        {
            PickupLocation = pickupLocation,
            PickupLocationType = pickupLocationType,
            PickupDate = new DateOnly(2026, 7, 1),
            ReturnDate = new DateOnly(2026, 7, 3),
            Category = VehicleCategory.Compact
        };
    }
}