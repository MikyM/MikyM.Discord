using DSharpPlus;
using DSharpPlus.EventArgs;
using MikyM.Discord.Attributes;

namespace MikyM.Discord.Tests;

public class SessionCreatedEventArgsSubscriberNone : IDiscordEventSubscriber<SessionCreatedEventArgs>
{
    public Task OnEventAsync(DiscordClient sender, SessionCreatedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}

[DiscordSubscriberResolvedBy(ResolveStrategy.Implementation)]
public class SessionCreatedEventArgsSubscriberImplStr : IDiscordEventSubscriber<SessionCreatedEventArgs>
{
    public Task OnEventAsync(DiscordClient sender, SessionCreatedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}

[DiscordSubscriberResolvedBy(ResolveStrategy.KeyedInterface)]
public class SessionCreatedEventArgsSubscriberKeyedInterface : IDiscordEventSubscriber<SessionCreatedEventArgs>
{
    public Task OnEventAsync(DiscordClient sender, SessionCreatedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}
