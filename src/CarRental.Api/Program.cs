using CarRental.Api.Endpoints;
using CarRental.Core.Domain;
using CarRental.Core.Pricing;
using CarRental.Core.Providers;
using CarRental.Core.Services;
using CarRental.Core.Storage;
using CarRental.Core.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BudgetWheelsPricingCalculator>();
builder.Services.AddSingleton<PremiumDrivePricingCalculator>();

builder.Services.AddSingleton<ICarRentalProvider, BudgetWheelsProvider>();
builder.Services.AddSingleton<ICarRentalProvider, PremiumDriveProvider>();
builder.Services.AddSingleton<ICarSearchService, CarSearchService>();

builder.Services.AddSingleton<IValidator<SearchCriteria>, SearchCriteriaValidator>();
builder.Services.AddSingleton<IValidator<Booking>, BookingValidator>();

builder.Services.AddSingleton<IBookingStore, InMemoryBookingStore>();
builder.Services.AddSingleton<IBookingReferenceGenerator, GuidBookingReferenceGenerator>();
builder.Services.AddSingleton<IBookingService, BookingService>();

var app = builder.Build();

app.MapGet("/", () => "Car Rental API");
app.MapCarsEndpoints();

app.Run();

public partial class Program;