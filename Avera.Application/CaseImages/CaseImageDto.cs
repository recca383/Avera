namespace Avera.Application.CaseImages
{
    public sealed class CaseImageDto
    {
            public int Index { get; set; }
            public DateTime UploadedAt { get; set; }
            public float Size { get; set; }
            public string MimeType { get; set; } = "image/jpeg";
    }
}