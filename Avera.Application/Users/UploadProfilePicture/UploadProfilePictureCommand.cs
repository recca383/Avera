using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Avera.Application.Users.UploadProfilePicture
{
    public sealed record UploadProfilePictureCommand(string MimeType, string fileName, Stream File) : ICommand;
}
