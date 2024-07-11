using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikyM.Discord.Attributes;
using MikyM.Discord.Util;

namespace MikyM.Discord;

/// <summary>
/// The metadata of the subscriber.
/// </summary>
internal record SubscriberMetadata
{
    public ResolveStrategy ResolveStrategy { get; }
    public Type ImplementationType { get; }
    public IReadOnlyDictionary<Type,(Type InterfaceType, SubscriberType Type)> ImplementedInterfacesInfo { get; }
    public IReadOnlyList<Type> EventTypes { get; }
    public bool IsCommandEventSubscriber { get; }
    public bool IsBaseEventSubscriber { get; }
    public int? Order { get; }

    private SubscriberMetadata(ResolveStrategy resolveStrategy, Type implementationType, IReadOnlyDictionary<Type,(Type InterfaceType, SubscriberType Type)> implementedInterfacesInfo, Type[] eventTypes, int? order)
    {
        ResolveStrategy = resolveStrategy;
        ImplementationType = implementationType;
        Order = order;
        ImplementedInterfacesInfo = implementedInterfacesInfo.ToDictionary().AsReadOnly();
        EventTypes = eventTypes.ToList().AsReadOnly();
        
        IsCommandEventSubscriber = implementedInterfacesInfo.Any(x => x.Value.Type is SubscriberType.Command);
        IsBaseEventSubscriber = implementedInterfacesInfo.Any(x => x.Value.Type is SubscriberType.Basic);
    }
    
    internal static SubscriberMetadata Create(Type serviceType, Type implementation)
    {
        var closedGenericInterfaces = implementation.GetInterfaces().Where(x => x.GetGenericArguments().Length == 1 && 
            x.IsGenericType && x.GetInterface(nameof(IDiscordBasicEventSubscriber)) is not null).Distinct().ToArray();

        var interfaceType = closedGenericInterfaces.First(x => x == serviceType);

        var dictionaryOfInterfaces = new Dictionary<Type, (Type InterfaceType, SubscriberType Type)>();
        
        var eventType = interfaceType.GetGenericArguments().First();
        
        var eventTypeEnum = TypeHelper.GetEventType(eventType);
        
        dictionaryOfInterfaces.Add(interfaceType.GetGenericArguments().First(), (eventType, eventTypeEnum switch
        {
            EventType.Basic => SubscriberType.Basic,
            EventType.Command => SubscriberType.Command,
            _ => throw new InvalidOperationException($"Invalid event type {eventType.Name}")
        }));
        
        var resolveStrategyAttribute = implementation.GetCustomAttribute<DiscordSubscriberResolvedByAttribute>();
        
        var resolveStrategy = resolveStrategyAttribute?.ResolveStrategy ?? ResolveStrategy.Implementation;
        
        var orderAttribute = implementation.GetCustomAttribute<DiscordSubscriberOrderAttribute>();
        
        var order = orderAttribute?.Order ?? int.MaxValue;
        
        return new SubscriberMetadata(resolveStrategy, implementation, dictionaryOfInterfaces, [eventType], order);
    }
    
    internal static SubscriberMetadata Create(Type implementation)
    {
        var closedGenericInterfaces = implementation.GetInterfaces().Where(x => x.GetGenericArguments().Length == 1 && 
            x.IsGenericType && x.GetInterface(nameof(IDiscordBasicEventSubscriber)) is not null).Distinct().ToArray();
        
        var dictionaryOfInterfaces = closedGenericInterfaces.ToDictionary(x => x.GetGenericArguments().First(), x =>
        {
            var arg = x.GetGenericArguments().First();

            var subscriberType = TypeHelper.GetBasicEventTypes().Contains(arg)
                ? SubscriberType.Basic
                : TypeHelper.GetCommandEventTypes().Contains(arg)
                    ? SubscriberType.Command
                    : throw new InvalidOperationException($"Invalid event type {arg.Name}");
            
            return (arg, subscriberType);
        });
        
        var eventTypes = closedGenericInterfaces.Select(x => x.GetGenericArguments().First()).ToArray();
        
        var resolveStrategyAttribute = implementation.GetCustomAttribute<DiscordSubscriberResolvedByAttribute>();
        
        var resolveStrategy = resolveStrategyAttribute?.ResolveStrategy ?? ResolveStrategy.Implementation;
        
        var orderAttribute = implementation.GetCustomAttribute<DiscordSubscriberOrderAttribute>();
        
        var order = orderAttribute?.Order ?? int.MaxValue;
        
        return new SubscriberMetadata(resolveStrategy, implementation, dictionaryOfInterfaces, eventTypes, order);
    }

    public override int GetHashCode()
    {
        return ImplementationType.GetHashCode();
    }

    public virtual bool Equals(SubscriberMetadata? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ImplementationType == other.ImplementationType;
    }
}
