

using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Users.SetDailyLimit;

public sealed record SetUserDailyLimitCommand(Guid UserId, int? DailyLimit) : ICommand;
