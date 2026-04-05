using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Application.CaseImages
{
    public sealed class CaseImage
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public ImageType Type { get; set; }
        public string ImageUrl { get; set; }
        public string StorageKey { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
