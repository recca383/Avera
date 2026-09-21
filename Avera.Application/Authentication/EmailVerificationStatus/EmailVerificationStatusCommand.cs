using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.EmailVerificationStatus
{
    public sealed record GetEmailVerificationStatusQuery
        : IQuery<EmailVerificationStatusResponse>;
}