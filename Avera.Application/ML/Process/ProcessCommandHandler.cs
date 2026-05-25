using System.Windows.Input;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.ML;
using Avera.Domain.Application.CaseImages;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.ML.Process
{
    internal sealed class ProcessCommandHandler(
        IApplicationDbContext dbContext,
        IMLService mLService,
        ILogger<ProcessCommandHandler> logger) : ICommandHandler<ProcessCommand, ProcessResponse>
    {
        public async Task<Result<ProcessResponse>> Handle(ProcessCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing ML for Case with Id: {CaseId}", command.CaseId);
            var selectedCase = await dbContext.Cases.FindAsync(new object[] { command.CaseId }, cancellationToken);

            logger.LogInformation("Selected Case with Id: {CaseId} has {NumberOfImages} images", selectedCase!.Id, selectedCase.CaseImages.Count);
            var processRequest = new ProcessRequest(
                CaseName: selectedCase!.CaseCode,
                OutputBlobName: $"{selectedCase.Id}.json",
                QuestionedImageUrl: selectedCase.CaseImages
                                    .Where(ci => ci.Type == ImageType.Suspected)
                                    .Select(ci => ci.FileName)
                                    .FirstOrDefault()!,
                ReferenceImageUrls: selectedCase.CaseImages
                                    .Where(ci => ci.Type == ImageType.Reference)
                                    .Select(ci => ci.FileName)
                                    .ToList()
            );

            logger.LogInformation("Sending process request for Case with Id: {CaseId}", selectedCase!.Id);  
            var processResponse = await mLService.ProcessAsync(processRequest, cancellationToken);

            logger.LogInformation("Received process response for Case with Id: {CaseId} with status: {Status}", selectedCase!.Id, processResponse);
            return processResponse;
        }
    }
}