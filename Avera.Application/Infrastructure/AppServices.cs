using System;

namespace Avera.Application.Infrastructure;

// Minimal static accessor to IServiceProvider for places where constructor DI isn't available.
// Prefer constructor injection; this is a pragmatic fallback for the command handler.
public static class AppServices
{
    public static IServiceProvider? ServiceProvider { get; set; }
}
