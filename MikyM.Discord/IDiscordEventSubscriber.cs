using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Commands;
using DSharpPlus.EventArgs;

namespace MikyM.Discord;

/// <summary>
/// Represents a subscriber for Discord events.
/// </summary>
[PublicAPI]
public interface IDiscordCommandEventSubscriber<in TEvent> : IDiscordBasicEventSubscriber<CommandsExtension, TEvent> where TEvent : AsyncEventArgs
{

}

/// <summary>
/// Represents a subscriber for Discord events.
/// </summary>
[PublicAPI]
public interface IDiscordEventSubscriber<in TEvent> : IDiscordBasicEventSubscriber<DiscordClient, TEvent> where TEvent : DiscordEventArgs
{
}

/// <summary>
/// Represents a subscriber for Discord events.
/// </summary>
[PublicAPI]
public interface IDiscordBasicEventSubscriber<in TSender, in TEvent> : IDiscordBasicEventSubscriber where TEvent : AsyncEventArgs where TSender : class
{
    /// <summary>
    /// Action to be executed when an event is received.
    /// </summary>
    /// <param name="sender">The client.</param>
    /// <param name="eventData">The event data.</param>
    /// <returns> A task that represent an asynchronous operation. </returns>
    Task OnEventAsync(TSender sender, TEvent eventData);
}

/// <summary>
/// Represents a subscriber for Discord events.
/// </summary>
[PublicAPI]
public interface IDiscordBasicEventSubscriber
{
}

