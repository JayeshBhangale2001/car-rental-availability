using CarRental.Core.Domain;
using CarRental.Core.ReferenceData;

namespace CarRental.Core.Validation;

public sealed class SearchCriteriaValidator : IValidator<SearchCriteria>
{
    private readonly IPickupLocationCatalog pickupLocationCatalog;

    public SearchCriteriaValidator(IPickupLocationCatalog pickupLocationCatalog)
    {
        this.pickupLocationCatalog = pickupLocationCatalog;
    }

    public ValidationResult Validate(SearchCriteria model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var issues = new List<ValidationIssue>();

        if (string.IsNullOrWhiteSpace(model.PickupLocation))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "pickupLocation",
                "search.pickup.required",
                "Pickup location is required."));
        }
        else
        {
            if (!pickupLocationCatalog.TryGetLocationType(model.PickupLocation, out var expectedType))
            {
                issues.Add(new ValidationIssue(
                    ValidationIssueKind.Input,
                    "pickupLocation",
                    "search.pickup.unsupported",
                    "Pickup location is not supported."));
            }
            else if (model.PickupLocationType != expectedType)
            {
                issues.Add(new ValidationIssue(
                    ValidationIssueKind.Input,
                    "pickupLocationType",
                    "search.pickup.typeMismatch",
                    "Pickup location type does not match the selected pickup location."));
            }
        }

        if (model.ReturnDate <= model.PickupDate)
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "dates",
                "search.dates.invalidRange",
                "Return date must be after pickup date."));
        }

        if (!Enum.IsDefined(model.PickupLocationType))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "pickupLocationType",
                "search.pickup.typeInvalid",
                "Pickup location type is invalid."));
        }

        if (model.Category is not null && !Enum.IsDefined(model.Category.Value))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "category",
                "search.category.invalid",
                "Vehicle category is invalid."));
        }

        return issues.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(issues);
    }
}