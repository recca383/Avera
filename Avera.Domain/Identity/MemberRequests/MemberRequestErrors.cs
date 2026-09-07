using Avera.Domain.Identity.Users;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Identity.MemberRequests
{
    public sealed record MemberRequestErrors : Error
    {
        private MemberRequestErrors(string code, string description, ErrorType errorType)
            : base(code, description, errorType) { }

        // Not Found

        public static MemberRequestErrors MemberRequestNotFound => new MemberRequestErrors(
            "MemberRequest.NotFound",
            "Member request not found",
            ErrorType.NotFound
        );
    }
}
