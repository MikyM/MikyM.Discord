using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using MikyM.Discord.Attributes;

namespace MikyM.Discord.Tests;

public class CommandExecutedEventArgsSubscriberNone : IDiscordCommandEventSubscriber<CommandExecutedEventArgs>
{
    public Task OnEventAsync(CommandsExtension sender, CommandExecutedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}

[DiscordSubscriberResolvedBy(ResolveStrategy.Implementation)]
public class CommandExecutedEventArgsSubscriberImpl : IDiscordCommandEventSubscriber<CommandExecutedEventArgs>
{
    public Task OnEventAsync(CommandsExtension sender, CommandExecutedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}

[DiscordSubscriberResolvedBy(ResolveStrategy.KeyedInterface)]
public class CommandExecutedEventArgsSubscriberKeyedInterface : IDiscordCommandEventSubscriber<CommandExecutedEventArgs>
{
    public Task OnEventAsync(CommandsExtension sender, CommandExecutedEventArgs eventData)
    {
        return Task.CompletedTask;
    }
}
