using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Application.Abstractions.Authentication
{
    public interface IUserContext
    {
        Guid UserId { get; }
        Guid TenantId { get; }
    }
}
