using CarRental.Core.Domain;

namespace CarRental.Core.Validation;

public sealed class BookingValidator : IValidator<Booking>
{
    public ValidationResult Validate(Booking model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var issues = new List<ValidationIssue>();

        if (string.IsNullOrWhiteSpace(model.DriverName))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "driverName",
                "booking.driverName.required",
                "Driver name is required."));
        }

        if (string.IsNullOrWhiteSpace(model.DocumentNumber))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "documentNumber",
                "booking.documentNumber.required",
                "Document number is required."));
        }

        if (!Enum.IsDefined(model.PickupLocationType))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "pickupLocationType",
                "booking.pickup.typeInvalid",
                "Pickup location type is invalid."));
        }

        if (!Enum.IsDefined(model.DocumentType))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "documentType",
                "booking.document.typeInvalid",
                "Document type is invalid."));
        }

        if (string.IsNullOrWhiteSpace(model.PickupLocation))
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "pickupLocation",
                "booking.pickup.required",
                "Pickup location is required."));
        }
        else
        {
            if (!SupportedPickupLocations.TryGetLocationType(model.PickupLocation, out var expectedType))
            {
                issues.Add(new ValidationIssue(
                    ValidationIssueKind.Input,
                    "pickupLocation",
                    "booking.pickup.unsupported",
                    "Pickup location is not supported."));
            }
            else
            {
                if (model.PickupLocationType != expectedType)
                {
                    issues.Add(new ValidationIssue(
                        ValidationIssueKind.Input,
                        "pickupLocationType",
                        "booking.pickup.typeMismatch",
                        "Pickup location type does not match the selected pickup location."));
                }

                if (!IsDocumentTypeValidForPickupType(model.DocumentType, expectedType))
                {
                    issues.Add(new ValidationIssue(
                        ValidationIssueKind.BusinessRule,
                        "documentType",
                        "booking.document.mismatch",
                        expectedType == PickupLocationType.Domestic
                            ? "Domestic pickup requires National ID."
                            : "International pickup requires Passport."));
                }
            }
        }

        if (model.ReturnDate <= model.PickupDate)
        {
            issues.Add(new ValidationIssue(
                ValidationIssueKind.Input,
                "dates",
                "booking.dates.invalidRange",
                "Return date must be after pickup date."));
        }

        return issues.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(issues);
    }

    private static bool IsDocumentTypeValidForPickupType(DocumentType documentType, PickupLocationType pickupLocationType)
    {
        return pickupLocationType switch
        {
            PickupLocationType.Domestic => documentType == DocumentType.NationalId,
            PickupLocationType.International => documentType == DocumentType.Passport,
            _ => false
        };
    }
}