using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CarRental.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarRental.Tests.Integration;

public class CarsEndpointsIntegrationTests
{
    [Fact]
    public async Task Search_Returns200AndOffers_WhenRequestIsValid()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var response = await client.GetAsync("/cars/search?pickup=Mumbai&from=2026-07-01&to=2026-07-04");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var offers = await response.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(offers);
        Assert.NotEmpty(offers!);
    }

    [Fact]
    public async Task Search_Returns400_WhenDateRangeIsInvalid()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var response = await client.GetAsync("/cars/search?pickup=Mumbai&from=2026-07-04&to=2026-07-04");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errorCode = body.GetProperty("errors")[0].GetProperty("code").GetString();
        Assert.Equal("search.dates.invalidRange", errorCode);
    }

    [Fact]
    public async Task Book_Returns201_WhenRequestIsValid()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var selectedOffer = await GetFirstOfferAsync(client, "Mumbai", "2026-07-01", "2026-07-04");

        var request = new
        {
            provider = selectedOffer.Provider,
            offerId = selectedOffer.OfferId,
            driverName = "Jayesh",
            documentType = "NationalId",
            documentNumber = "NID-12345",
            pickup = "Mumbai",
            from = "2026-07-01",
            to = "2026-07-04"
        };

        var response = await client.PostAsJsonAsync("/cars/book", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var reference = body.GetProperty("reference").GetString();
        Assert.False(string.IsNullOrWhiteSpace(reference));
    }

    [Fact]
    public async Task Book_Returns400_WhenPickupLocationIsUnsupported()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var request = new
        {
            provider = "PremiumDrive",
            offerId = "PD-ECON-001",
            driverName = "Jayesh",
            documentType = "NationalId",
            documentNumber = "NID-12345",
            pickup = "Pune",
            from = "2026-07-01",
            to = "2026-07-04"
        };

        var response = await client.PostAsJsonAsync("/cars/book", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errorCode = body.GetProperty("errors")[0].GetProperty("code").GetString();
        Assert.Equal("search.pickup.unsupported", errorCode);
    }

    [Fact]
    public async Task Book_Returns422_WhenDocumentTypeViolatesPickupRule()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var selectedOffer = await GetFirstOfferAsync(client, "Mumbai", "2026-07-01", "2026-07-04");

        var request = new
        {
            provider = selectedOffer.Provider,
            offerId = selectedOffer.OfferId,
            driverName = "Jayesh",
            documentType = "Passport",
            documentNumber = "P-12345",
            pickup = "Mumbai",
            from = "2026-07-01",
            to = "2026-07-04"
        };

        var response = await client.PostAsJsonAsync("/cars/book", request);

        Assert.Equal((HttpStatusCode)422, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var errorCode = body.GetProperty("errors")[0].GetProperty("code").GetString();
        Assert.Equal("booking.document.mismatch", errorCode);
    }

    [Fact]
    public async Task BookingLookup_Returns200_WhenReferenceExists()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var selectedOffer = await GetFirstOfferAsync(client, "Mumbai", "2026-07-01", "2026-07-04");
        var bookRequest = new
        {
            provider = selectedOffer.Provider,
            offerId = selectedOffer.OfferId,
            driverName = "Jayesh",
            documentType = "NationalId",
            documentNumber = "NID-12345",
            pickup = "Mumbai",
            from = "2026-07-01",
            to = "2026-07-04"
        };

        var bookResponse = await client.PostAsJsonAsync("/cars/book", bookRequest);
        var bookBody = await bookResponse.Content.ReadFromJsonAsync<JsonElement>();
        var reference = bookBody.GetProperty("reference").GetString();

        var lookupResponse = await client.GetAsync($"/cars/booking/{reference}");

        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
    }

    [Fact]
    public async Task BookingLookup_Returns404_WhenReferenceDoesNotExist()
    {
        using var testClient = CreateTestClient();
        var client = testClient.Client;

        var response = await client.GetAsync("/cars/booking/BK-DOES-NOT-EXIST");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<(string Provider, string OfferId)> GetFirstOfferAsync(
        HttpClient client,
        string pickup,
        string from,
        string to)
    {
        var searchResponse = await client.GetAsync($"/cars/search?pickup={pickup}&from={from}&to={to}");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);

        var offers = await searchResponse.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(offers);
        Assert.NotEmpty(offers!);

        var firstOffer = offers![0];
        return (
            Provider: firstOffer.GetProperty("provider").GetString()!,
            OfferId: firstOffer.GetProperty("offerId").GetString()!);
    }

    private static TestClientContext CreateTestClient()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        return new TestClientContext(factory, client);
    }

    private sealed class TestClientContext : IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;

        public TestClientContext(WebApplicationFactory<Program> factory, HttpClient client)
        {
            this.factory = factory;
            Client = client;
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            factory.Dispose();
        }
    }
}