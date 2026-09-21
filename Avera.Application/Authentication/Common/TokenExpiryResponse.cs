namespace Avera.Application.Authentication.Common;

public sealed record TokenExpiryResponse(
    int ExpiresInMinutes
);