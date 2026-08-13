using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedKernel;

namespace Avera.Application.Cases.Create
{
    public sealed class CreateCaseCommandHandler(
        IApplicationDbContext dbContext,
        IUserContext userContext) 
        : ICommandHandler<CreateCaseCommand, Case>
    {
        private static readonly ILogger logger = Log.ForContext<CreateCaseCommandHandler>();
        
        public async Task<Result<Case>> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
        {
            if(userContext.TenantId == null)
                return Result.Failure<Case>(TenantErrors.NotMember);

            logger.Information("Creating a new case for subject: {SubjectName}", command.SubjectName);
            var newCase = new Case()
            {
                Id = Guid.NewGuid(),
                CaseCode = GenerateCaseCode(),
                SubjectName = command.SubjectName,
                CreatedByUserId = userContext.UserId,
                TenantId = userContext.TenantId,
                AnalysisType = command.AnalysisType,
                Priority = command.Priority,
                Notes = "",
                CreatedAt = DateTime.UtcNow,
                Status = Status.Processing
            };         

            if (await dbContext.Cases.AnyAsync(c => c.CaseCode == newCase.CaseCode, cancellationToken))
            {
                logger.Warning("A case with code: {CaseCode} already exists", newCase.CaseCode);
                return Result.Failure<Case>(CaseErrors.CaseAlreadyExists);
            }
            
            logger.Information("Adding new case to database: {CaseCode}", newCase.CaseCode);

            await dbContext.Cases.AddAsync(newCase, cancellationToken);

            logger.Information("New case added to database: {CaseCode}", newCase.CaseCode);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.Information("Case created successfully with ID: {CaseId}", newCase.Id);
            
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