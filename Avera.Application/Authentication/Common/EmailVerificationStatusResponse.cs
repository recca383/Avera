using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.Common
{
    public sealed record EmailVerificationStatusResponse(
        bool EmailConfirmed
    );
}
