using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;

namespace MikyM.Discord.Util;

internal static class TypeHelper
{
    private static readonly IReadOnlyList<Type> BasicEventTypes =  typeof(ChannelCreatedEventArgs).Assembly.GetTypes()
        .Where(x => x is { IsClass: true, IsAbstract: false } && x.BaseType == typeof(DiscordEventArgs))
        .ToList().AsReadOnly();

    internal static IReadOnlyList<Type> GetBasicEventTypes()
        => BasicEventTypes;
    
    private static readonly IReadOnlyList<Type> CommandEventTypes = typeof(CommandExecutedEventArgs).Assembly.GetTypes()
        .Where(x => x is { IsClass: true, IsAbstract: false } && x.BaseType == typeof(AsyncEventArgs))
        .ToList().AsReadOnly();

    internal static IReadOnlyList<Type> GetCommandEventTypes()
        => CommandEventTypes;
    
    internal static EventType GetEventType(Type type)
    {
        if (BasicEventTypes.Contains(type))
        {
            return EventType.Basic;
        }

        if (CommandEventTypes.Contains(type))
        {
            return EventType.Command;
        }

        throw new InvalidOperationException($"Invalid event type {type.Name}");
    }
}
