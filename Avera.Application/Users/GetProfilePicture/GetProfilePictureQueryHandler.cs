using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Users.GetProfilePicture
{
    internal sealed class GetProfilePictureQueryHandler(
        IUserContext userContext,
        IBlobStorageService blobStorageService,
        UserManager<User> userManager
        ) : IQueryHandler<GetProfilePictureQuery, GetProfileProfileQueryResponse>
    {
        public async Task<Result<GetProfileProfileQueryResponse>> Handle(GetProfilePictureQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetProfileProfileQueryResponse>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetProfileProfileQueryResponse>(UserErrors.IsSuspended);

            var stream = await blobStorageService.DownloadAsync(user.ProfilePictureBlob, cancellationToken);

            if (stream == null)
            {
                return Result.Failure<GetProfileProfileQueryResponse>(UserErrors.ProfilePictureNotFound);
            }

            var contentType = user.ProfilePictureBlob!.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? "image/png"
                : "application/octet-stream";

            var response = new GetProfileProfileQueryResponse(stream, contentType);

            return Result.Success(response);
        }
    }
}
