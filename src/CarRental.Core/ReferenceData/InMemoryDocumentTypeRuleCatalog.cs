using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public sealed class InMemoryDocumentTypeRuleCatalog : IDocumentTypeRuleCatalog
{
    private static readonly IReadOnlyDictionary<PickupLocationType, DocumentRule> Rules =
        new Dictionary<PickupLocationType, DocumentRule>
        {
            [PickupLocationType.Domestic] = new DocumentRule(
                DocumentType.NationalId,
                "Domestic pickup requires National ID."),
            [PickupLocationType.International] = new DocumentRule(
                DocumentType.Passport,
                "International pickup requires Passport.")
        };

    public bool IsDocumentTypeValidForPickupType(DocumentType documentType, PickupLocationType pickupLocationType)
    {
        return Rules.TryGetValue(pickupLocationType, out var rule)
            && rule.RequiredDocumentType == documentType;
    }

    public string GetDocumentMismatchMessage(PickupLocationType pickupLocationType)
    {
        return Rules.TryGetValue(pickupLocationType, out var rule)
            ? rule.MismatchMessage
            : "Document type does not match pickup location type.";
    }

    private sealed record DocumentRule(DocumentType RequiredDocumentType, string MismatchMessage);
}
