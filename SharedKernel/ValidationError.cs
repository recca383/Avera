namespace SharedKernel;

public record ValidationError : Error
{
    public ValidationError(Error[] errors, string? code = null, string? description = null)
        : base(
            code ?? "Validation.General",
            description ?? "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
