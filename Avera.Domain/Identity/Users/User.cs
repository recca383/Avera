using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Identity.Users
{
    public class User : IdentityUser
    {
        // Navigation 
        public Guid TenantId { get; set; }
    }
}
