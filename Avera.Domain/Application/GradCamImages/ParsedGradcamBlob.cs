namespace Avera.Domain.Application.OverlayImages
{
    public sealed record ParsedGradcamBlob(
        GradCamSlot Slot,
        GradCamVariant Type,
        string BlobPath
    );
    
}