using SharedKernel;

namespace Avera.Domain.Application.CaseImages
{
    public record CaseImageErrors : Error
     {
        private CaseImageErrors(string code, string message, ErrorType errorType) : base(code, message, errorType)
        {
        }

        public static CaseImageErrors CaseImageNotFound => new CaseImageErrors(
            "case_image_not_found",
            "The specified case image was not found.",
            ErrorType.NotFound);

        public static CaseImageErrors ImageUploadFailed => new CaseImageErrors(
            "image_upload_failed",
            "Failed to upload the image.",
            ErrorType.ServerError);
         
    }
}