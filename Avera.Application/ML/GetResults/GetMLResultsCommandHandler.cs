using System.Windows.Input;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.Cases;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.ML.GetResults
{
    internal sealed class GetMLResultsCommandHandler(
        IApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        ILogger<GetMLResultsCommandHandler> logger
    ) : ICommandHandler<GetMLResultsCommand, GetMLResultsResponse>
    {
        public async Task<Result<GetMLResultsResponse>> Handle(GetMLResultsCommand command, CancellationToken cancellationToken)
        {
            var selectedCase = dbContext.Cases.FirstOrDefault(c => c.Id == command.CaseId);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with Id: {CaseId} not found", command.CaseId);
                return Result.Failure<GetMLResultsResponse>(CaseErrors.CaseNotFound);
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