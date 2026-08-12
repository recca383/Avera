

using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.JoinInviteCode
{
    public sealed record JoinInviteCodeCommand (
        string InviteCode
    ): ICommand;
}