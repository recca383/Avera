using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Application.Cases;

namespace Avera.Domain.Application.CaseImages
{
    public sealed class CaseImage
    {
        public Guid Id { get; set; }
        public int Index { get; set;}    
        public string MimeType { get; set; } = "image/jpeg";
        public float Size { get; set; }
        public ImageType Type { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedById { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties 
        public Case Case{ get; set; } = null!;
        public Guid CaseId { get; set; }

        public string FileName => Type switch 
        {
            ImageType.Suspected => $"F{Index}.{MimeType.Split('/').Last()}",
            ImageType.Reference => $"G{Index}.{MimeType.Split('/').Last()}",
            _ => throw new InvalidOperationException("Invalid image type")
        };

        public string BlobName => $"{Case.CaseCode}/{FileName}";
    }
}