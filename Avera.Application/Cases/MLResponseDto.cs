using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases
{
    public sealed record MLResponseDto(
        float ConfidenceForged,
        float ConfidenceGenuine,
        float Distance,
        List<GradCamDto> GradCamResults,
        float Threshold,
        string Verdict
    );
}
