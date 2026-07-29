var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Car Rental API");

app.Run();

public partial class Program;