using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Cases
{
    public class MLResponse
    {
        public float ConfidenceForged { get; init; }

        public float ConfidenceGenuine { get; init; }

        public float Distance { get; init; }

        public List<string> GradcamBlobId { get; init; }
        public float Threshold { get; init; }
        public string Verdict { get; init; }
    }
}
