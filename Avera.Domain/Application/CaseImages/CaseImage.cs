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
        public ImageType Type { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation Properties 
        public Case? Case { get; set; }
        public Guid CaseId { get; set; }
    
    }
}