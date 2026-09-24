using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Users.GetProfilePicture
{
    public record GetProfileProfileQueryResponse(Stream ImageStream, string ContentType);
}
