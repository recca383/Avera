using SharedKernel;

namespace Avera.Domain.Application.Cases
{
    public record CaseErrors : Error
    {
        private CaseErrors(string code, string message, ErrorType errorType) : base(code, message, errorType)
        {
        }

        public static CaseErrors CaseNotFound => new CaseErrors(
            "case_not_found",
            "The specified case was not found.",
            ErrorType.NotFound);

        public static CaseErrors CaseAlreadyExists => new CaseErrors(
            "case_already_exists",
            "A case with the specified identifier already exists.",
            ErrorType.Conflict);
        
        public static CaseErrors CaseAlreadyAtStatus => new CaseErrors(
            "case_already_at_status",
            "The case is already at the specified status.",
            ErrorType.Conflict);

        public static CaseErrors CaseAlreadyReviewed => new CaseErrors(
            "Case.AlreadyReviewed",
            "The case is already reviewed.",
            ErrorType.Conflict);

        public static CaseErrors MLResultsNotFound => new CaseErrors(
            "ml_results_not_found",
            "ML results for the specified case were not found.",
            ErrorType.NotFound);

        public static CaseErrors PdfExportNotAllowed => new CaseErrors(
            "Case.PdfExportNotAllowed",
            "The Pdf Export is not allowed by the admin",
            ErrorType.Problem);
    }
}