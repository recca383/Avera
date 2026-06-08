using Avera.Domain.Application.Cases;

namespace Avera.Domain.Application.OverlayImages
{
    public sealed class GradCamImage
    {
         public Guid Id { get; set; }
         public Guid CaseId { get; set; }
         public GradCamSlot Slot { get; set;}
         public GradCamVariant Type { get; set; }
         public string BlobPath { get; set; } = string.Empty;
         
         // Navigation property
         public Case? Case { get; set; }
    }
}