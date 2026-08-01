using CarRental.Core.Domain;

namespace CarRental.Core.ReferenceData;

public interface IDocumentTypeRuleCatalog
{
    bool IsDocumentTypeValidForPickupType(DocumentType documentType, PickupLocationType pickupLocationType);

    string GetDocumentMismatchMessage(PickupLocationType pickupLocationType);
}
