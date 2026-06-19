using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.Register
{
    public sealed record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role
    ) : ICommand;
}