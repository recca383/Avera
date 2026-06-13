namespace Avera.Application.Authentication.Login
{
    public sealed record LoginResponse(
        string AccessToken,
        DateTime ExpiresAt);
}