using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Cases.Create
{
    public sealed class CreateCaseCommandHandler(
        IApplicationDbContext dbContext,
        ILogger<CreateCaseCommandHandler> logger) : ICommandHandler<CreateCaseCommand, Case>
    {
        public async Task<Result<Case>> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Creating a new case for subject: {SubjectName}", command.SubjectName);
            var newCase = new Case()
            {
                Id = Guid.NewGuid(),
                CaseCode = GenerateCaseCode(),
                SubjectName = command.SubjectName,
                UserId = Guid.Empty, // Temporary
                AnalysisType = command.AnalysisType,
                Priority = command.Priority,
                Notes = "",
                CreatedAt = DateTime.UtcNow,
                Status = Status.Processing
            };         

            if (await dbContext.Cases.AnyAsync(c => c.CaseCode == newCase.CaseCode, cancellationToken))
            {
                logger.LogWarning("A case with code: {CaseCode} already exists", newCase.CaseCode);
                return Result.Failure<Case>(CaseErrors.CaseAlreadyExists);
            }
            
            logger.LogInformation("Adding new case to database: {CaseCode}", newCase.CaseCode);

            await dbContext.Cases.AddAsync(newCase, cancellationToken);

            logger.LogInformation("New case added to database: {CaseCode}", newCase.CaseCode);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Case created successfully with ID: {CaseId}", newCase.Id);
            
            return Result.Success(newCase);
        }

        private string GenerateCaseCode()
        {
            var numofcasesToday = dbContext
                                    .Cases
                                    .Count(c => c.CreatedAt.Date == DateTime.UtcNow.Date) + 1;
            return $"CASE-{DateTime.UtcNow:MMddyyyy}-{numofcasesToday:D3}";
        }
    }
}