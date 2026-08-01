using System.Globalization;
using CarRental.Api.Contracts;
using CarRental.Core.Domain;
using CarRental.Core.Services;
using CarRental.Core.Validation;

namespace CarRental.Api.Endpoints;

public static class CarsEndpoints
{
    private const string DateFormat = "yyyy-MM-dd";

    public static RouteGroupBuilder MapCarsEndpoints(this IEndpointRouteBuilder app)
    {
        var cars = app.MapGroup("/cars")
            .WithTags("Cars");

        cars.MapGet("/pickup-locations", GetPickupLocations)
            .WithName("GetPickupLocations")
            .WithSummary("Get supported pickup locations")
            .WithDescription("Returns all supported pickup locations grouped by location type.")
            .Produces<PickupLocationResponseDto[]>(StatusCodes.Status200OK);

        cars.MapGet("/search", SearchAsync)
            .WithName("SearchCars")
            .WithSummary("Search available rental cars")
            .WithDescription("Queries providers and returns normalized, price-sorted available offers.")
            .Produces<SearchCarResponseDto[]>(StatusCodes.Status200OK)
            .Produces<ApiValidationErrorResponseDto>(StatusCodes.Status400BadRequest);

        cars.MapPost("/book", BookAsync)
            .WithName("BookCar")
            .WithSummary("Create a booking for a selected offer")
            .WithDescription("Validates input and document rules, then creates a booking with a generated reference.")
            .Produces<BookingConfirmationResponseDto>(StatusCodes.Status201Created)
            .Produces<ApiValidationErrorResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<ApiValidationErrorResponseDto>(StatusCodes.Status422UnprocessableEntity);

        cars.MapGet("/booking/{reference}", GetBookingByReferenceAsync)
            .WithName("GetBookingByReference")
            .WithSummary("Get booking details by reference")
            .WithDescription("Returns booking details when the reference exists.")
            .Produces<BookingDetailsResponseDto>(StatusCodes.Status200OK)
            .Produces<ApiValidationErrorResponseDto>(StatusCodes.Status400BadRequest)
            .Produces<BookingNotFoundResponseDto>(StatusCodes.Status404NotFound);

        return cars;
    }

    private static IResult GetPickupLocations()
    {
        var domestic = SupportedPickupLocations.GetDomesticLocations()
            .Select(location => new PickupLocationResponseDto
            {
                Name = location,
                LocationType = PickupLocationType.Domestic.ToString()
            });

        var international = SupportedPickupLocations.GetInternationalLocations()
            .Select(location => new PickupLocationResponseDto
            {
                Name = location,
                LocationType = PickupLocationType.International.ToString()
            });

        return Results.Ok(domestic.Concat(international).ToArray());
    }

    private static async Task<IResult> SearchAsync(
        [AsParameters] SearchCarsRequestDto request,
        ICarSearchService carSearchService,
        IValidator<SearchCriteria> searchValidator,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Pickup))
        {
            return BadRequest("pickup", "search.pickup.required", "Pickup location is required.");
        }

        if (!TryParseDateOnly(request.From, out var pickupDate, out var fromError))
        {
            return BadRequest("from", "search.from.invalid", fromError);
        }

        if (!TryParseDateOnly(request.To, out var returnDate, out var toError))
        {
            return BadRequest("to", "search.to.invalid", toError);
        }

        if (!TryParseVehicleCategory(request.Category, out var category, out var categoryError))
        {
            return BadRequest("category", "search.category.invalid", categoryError);
        }

        var hasPickupType = SupportedPickupLocations.TryGetLocationType(request.Pickup, out var pickupLocationType);
        var criteria = new SearchCriteria
        {
            PickupLocation = request.Pickup.Trim(),
            PickupLocationType = hasPickupType ? pickupLocationType : PickupLocationType.Domestic,
            PickupDate = pickupDate,
            ReturnDate = returnDate,
            Category = category
        };

        var validationResult = searchValidator.Validate(criteria);
        if (!validationResult.IsValid)
        {
            return ValidationFailure(validationResult);
        }

        var offers = await carSearchService.SearchAsync(criteria, cancellationToken);
        var response = offers
            .Select(offer => new SearchCarResponseDto
            {
                Provider = offer.ProviderName,
                OfferId = offer.OfferId,
                VehicleName = offer.VehicleName,
                Category = offer.Category.ToString(),
                PerDayRate = offer.PerDayRate,
                TotalPrice = offer.TotalPrice,
                CancellationPolicy = offer.CancellationPolicy,
                InsuranceIncluded = !string.IsNullOrWhiteSpace(offer.InsuranceType),
                Currency = offer.Currency
            })
            .ToArray();

        return Results.Ok(response);
    }

    private static async Task<IResult> BookAsync(
        BookCarRequestDto request,
        ICarSearchService carSearchService,
        IValidator<SearchCriteria> searchValidator,
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Provider))
        {
            return BadRequest("provider", "booking.provider.required", "Provider is required.");
        }

        if (string.IsNullOrWhiteSpace(request.OfferId))
        {
            return BadRequest("offerId", "booking.offer.required", "Offer ID is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Pickup))
        {
            return BadRequest("pickup", "booking.pickup.required", "Pickup location is required.");
        }

        if (!TryParseDateOnly(request.From, out var pickupDate, out var fromError))
        {
            return BadRequest("from", "booking.from.invalid", fromError);
        }

        if (!TryParseDateOnly(request.To, out var returnDate, out var toError))
        {
            return BadRequest("to", "booking.to.invalid", toError);
        }

        if (!TryParseDocumentType(request.DocumentType, out var documentType, out var documentTypeError))
        {
            return BadRequest("documentType", "booking.document.typeInvalid", documentTypeError);
        }

        var hasPickupType = SupportedPickupLocations.TryGetLocationType(request.Pickup, out var pickupLocationType);
        var searchCriteria = new SearchCriteria
        {
            PickupLocation = request.Pickup.Trim(),
            PickupLocationType = hasPickupType ? pickupLocationType : PickupLocationType.Domestic,
            PickupDate = pickupDate,
            ReturnDate = returnDate,
            Category = null
        };

        var searchValidationResult = searchValidator.Validate(searchCriteria);
        if (!searchValidationResult.IsValid)
        {
            return ValidationFailure(searchValidationResult);
        }

        var availableOffers = await carSearchService.SearchAsync(searchCriteria, cancellationToken);
        var selectedOffer = availableOffers.FirstOrDefault(offer =>
            string.Equals(offer.ProviderName, request.Provider.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(offer.OfferId, request.OfferId.Trim(), StringComparison.OrdinalIgnoreCase));

        if (selectedOffer is null)
        {
            return BadRequest(
                "offerId",
                "booking.offer.notFound",
                "The selected offer was not found for the provided search context.");
        }

        var booking = new Booking
        {
            ReferenceNumber = "TEMP",
            ProviderName = selectedOffer.ProviderName,
            DriverName = request.DriverName?.Trim() ?? string.Empty,
            DocumentType = documentType,
            DocumentNumber = request.DocumentNumber?.Trim() ?? string.Empty,
            PickupLocation = request.Pickup.Trim(),
            PickupLocationType = hasPickupType ? pickupLocationType : PickupLocationType.Domestic,
            PickupDate = pickupDate,
            ReturnDate = returnDate,
            SelectedOffer = selectedOffer,
            BookedAtUtc = default
        };

        var result = await bookingService.CreateBookingAsync(booking, cancellationToken);
        if (!result.IsSuccess)
        {
            return ValidationFailure(result.ValidationResult);
        }

        var confirmation = result.Confirmation!;
        var response = new BookingConfirmationResponseDto
        {
            Reference = confirmation.ReferenceNumber,
            Provider = confirmation.ProviderName,
            Category = confirmation.Category.ToString(),
            TotalPrice = confirmation.TotalPrice,
            CancellationPolicy = confirmation.CancellationPolicy
        };

        return Results.Created($"/cars/booking/{confirmation.ReferenceNumber}", response);
    }

    private static async Task<IResult> GetBookingByReferenceAsync(
        string reference,
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return BadRequest("reference", "booking.reference.required", "Booking reference is required.");
        }

        var booking = await bookingService.GetBookingByReferenceAsync(reference, cancellationToken);
        if (booking is null)
        {
            return Results.NotFound(new BookingNotFoundResponseDto("Booking reference was not found."));
        }

        var response = new BookingDetailsResponseDto
        {
            Reference = booking.ReferenceNumber,
            Provider = booking.ProviderName,
            DriverName = booking.DriverName,
            DocumentType = booking.DocumentType.ToString(),
            DocumentNumber = booking.DocumentNumber,
            Pickup = booking.PickupLocation,
            PickupLocationType = booking.PickupLocationType.ToString(),
            From = booking.PickupDate,
            To = booking.ReturnDate,
            Offer = new BookingOfferResponseDto
            {
                Provider = booking.SelectedOffer.ProviderName,
                OfferId = booking.SelectedOffer.OfferId,
                VehicleName = booking.SelectedOffer.VehicleName,
                Category = booking.SelectedOffer.Category.ToString(),
                PerDayRate = booking.SelectedOffer.PerDayRate,
                TotalPrice = booking.SelectedOffer.TotalPrice,
                CancellationPolicy = booking.SelectedOffer.CancellationPolicy,
                InsuranceType = booking.SelectedOffer.InsuranceType,
                InsuranceIncluded = !string.IsNullOrWhiteSpace(booking.SelectedOffer.InsuranceType),
                Currency = booking.SelectedOffer.Currency
            },
            BookedAtUtc = booking.BookedAtUtc
        };

        return Results.Ok(response);
    }

    private static bool TryParseDateOnly(string? value, out DateOnly date, out string error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            date = default;
            error = "Date is required and must use YYYY-MM-DD format.";
            return false;
        }

        if (!DateOnly.TryParseExact(value.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            error = "Date is invalid and must use YYYY-MM-DD format.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool TryParseVehicleCategory(string? value, out VehicleCategory? category, out string error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            category = null;
            error = string.Empty;
            return true;
        }

        if (Enum.TryParse<VehicleCategory>(value.Trim(), true, out var parsedCategory) &&
            Enum.IsDefined(parsedCategory))
        {
            category = parsedCategory;
            error = string.Empty;
            return true;
        }

        category = null;
        error = "Vehicle category is invalid.";
        return false;
    }

    private static bool TryParseDocumentType(string? value, out DocumentType documentType, out string error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            documentType = default;
            error = "Document type is required.";
            return false;
        }

        if (Enum.TryParse<DocumentType>(value.Trim(), true, out var parsedDocumentType) &&
            Enum.IsDefined(parsedDocumentType))
        {
            documentType = parsedDocumentType;
            error = string.Empty;
            return true;
        }

        documentType = default;
        error = "Document type is invalid.";
        return false;
    }

    private static IResult BadRequest(string field, string code, string message)
    {
        return Results.BadRequest(new ApiValidationErrorResponseDto(new[]
        {
            new ApiValidationIssueDto(
                Kind: ValidationIssueKind.Input.ToString(),
                Field: field,
                Code: code,
                Message: message)
        }));
    }

    private static IResult ValidationFailure(ValidationResult validationResult)
    {
        var statusCode = validationResult.Errors.Any(issue => issue.Kind == ValidationIssueKind.BusinessRule)
            ? StatusCodes.Status422UnprocessableEntity
            : StatusCodes.Status400BadRequest;

        var response = new ApiValidationErrorResponseDto(
            validationResult.Errors
                .Select(issue => new ApiValidationIssueDto(
                    Kind: issue.Kind.ToString(),
                    Field: issue.Field,
                    Code: issue.Code,
                    Message: issue.Message))
                .ToArray());

        return Results.Json(response, statusCode: statusCode);
    }
}