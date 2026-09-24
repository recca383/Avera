using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Users.UploadProfilePicture
{
    internal sealed class UploadProfilePictureCommandHandler
        (
            IUserContext userContext,
            IBlobStorageService blobStorage,
            IIdentityDbContext identityDbContext,
            UserManager<User> userManager
        ): ICommandHandler<UploadProfilePictureCommand>
    {
        public async Task<Result> Handle(UploadProfilePictureCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (user!.IsSuspended)
                return Result.Failure(UserErrors.IsSuspended);

            // If an image already exists for this case/type/index, delete it first (upsert behavior)
            var existingProfilePicture = string.IsNullOrEmpty(user.ProfilePictureBlob);

            if (!existingProfilePicture)
            {
                try
                {
                    await blobStorage.DeleteAsync(user.ProfilePictureBlob, cancellationToken);
                }
                catch (Exception ex)
                {
                    return Result.Failure(new Error("User.ProfilePicture", "Unable to replace/delete existing profile picture", ErrorType.Failure));
                }

                user.ProfilePictureBlob = "";
            }

            user.ProfilePictureBlob = GetBlobDirectory(user.Id, command.MimeType, command.fileName);

            try
            {
                await blobStorage.UploadFileAsync(
                    command.File,
                    user.ProfilePictureBlob,
                    command.MimeType,
                    cancellationToken
                );

                await identityDbContext.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("User.ProfilePicture", "Uploading Profile Picture Failed, please try again!", ErrorType.Failure));
            }


            return Result.Success();
        }

        private static string GetBlobDirectory(Guid UserId, string MimeType, string FileName) => $"ProfilePictures/{UserId}-{FileName}";
    }
   
}
