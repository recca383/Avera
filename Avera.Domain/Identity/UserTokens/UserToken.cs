using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Identity.UserTokens
{
    public sealed class UserToken : IdentityUserToken<Guid>
    {
    }
}
