using Avera.Domain.Application.OverlayImages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases
{
    public sealed record GradCamDto(
        GradCamSlot Slot,
        GradCamVariant Variant,
        Guid ImageId
        );
}
