using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Infrastructure.Configuration
{
    public sealed class AppOptions
    {
        public string PublicBaseUrl { get; set;  } = string.Empty;
        public string DeepLinkBase { get; set; } = string.Empty;
    }
}
