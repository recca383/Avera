using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Windows.Input;

namespace Avera.Application.ML.GetResults
{
    internal sealed class GetMLResultsCommandHandler(
        IApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        ILogger<GetMLResultsCommandHandler> logger,
        IUserContext userContext,
        UserManager<User> userManager
    ) : ICommandHandler<GetMLResultsCommand, GetMLResultsResponse>
    {
        public async Task<Result<GetMLResultsResponse>> Handle(GetMLResultsCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetMLResultsResponse>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetMLResultsResponse>(UserErrors.IsSuspended);

            var selectedCase = dbContext.Cases.FirstOrDefault(c => c.Id == command.CaseId);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with Id: {CaseId} not found", command.CaseId);
                return Result.Failure<GetMLResultsResponse>(CaseErrors.CaseNotFound);
            }

            if (!selectedCase.IsPdfExportAllowed)
            {
                return Result.Failure<GetMLResultsResponse>(CaseErrors.PdfExportNotAllowed);
            }

            var caseOutput = $"{selectedCase.CaseCode}/{Case.OutputBlob}";

            try
            {
                var mlResults = await blobStorageService.DownloadAsync(caseOutput, cancellationToken);

                var result = new GetMLResultsResponse(selectedCase.Id, mlResults!);

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error downloading ML results for Case with Id: {CaseId}", command.CaseId);
                return Result.Failure<GetMLResultsResponse>(CaseErrors.MLResultsNotFound);
            }
                 
        }
    }
}