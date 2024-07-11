using System;
using System.Collections.Generic;
using System.Linq;

namespace MikyM.Discord;

internal record DiscordEventMetadata
{
    public Type Event { get; }
    public EventType EventType { get; }
    public Dictionary<Type,SubscriberMetadata> Subscribers { get; }

    private DiscordEventMetadata(Type @event, EventType eventType, Dictionary<Type, SubscriberMetadata> subscribers)
    {
        Event = @event;
        EventType = eventType;
        Subscribers = subscribers.ToDictionary();
    }
    
    internal static DiscordEventMetadata Create(Type @event, EventType eventType, Dictionary<Type, SubscriberMetadata> subscribers)
    {
        return new DiscordEventMetadata(@event, eventType, subscribers);
    }
}
