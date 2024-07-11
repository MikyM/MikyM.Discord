using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Commands;
using DSharpPlus.EventArgs;
using MikyM.Discord.Util;

namespace MikyM.Discord;

/// <summary>
/// Represents the subscriber metadata provider.
/// </summary>
public class MetadataProvider
{
    private readonly Dictionary<Type,DiscordEventMetadata> _eventData = new();
    private readonly Dictionary<Type,DiscordEventMetadata> _basicEventData = new();
    private readonly Dictionary<Type,DiscordEventMetadata> _commandEventData = new();
    
    internal Dictionary<Type,DiscordEventMetadata> GetEventData() => _eventData;
    internal Dictionary<Type,DiscordEventMetadata> GetBasicEventData() => _basicEventData;
    internal Dictionary<Type,DiscordEventMetadata> GetCommandEventData() => _commandEventData;
    
    private readonly Dictionary<SubscriberDelegateKey, SubscriberDelegate> _delegateCache = new();
    
    internal Dictionary<SubscriberDelegateKey, SubscriberDelegate> GetSubscriberDelegates() => _delegateCache;
    
    internal SubscriberDelegate GetSubscriberDelegate(Type subscriberType, Type eventType)
    {
        return _delegateCache[SubscriberDelegateKey.Create(subscriberType, eventType)];
    }

    internal static MetadataProvider Instance { get; } = Create();
    internal static MetadataProvider Create()
    {
        return new MetadataProvider();
    }
    
    private MetadataProvider() { }

    private void PrepareAndCacheDelegates()
    {
        foreach (var (eventType, eventData) in GetBasicEventData())
        {
            foreach (var (implementationType, _) in eventData.Subscribers)
            {
                var key = SubscriberDelegateKey.Create(implementationType, eventType);
                
                if (_delegateCache.ContainsKey(key))
                {
                    continue;
                }
                
                var lambda = CreateDelegate<DiscordClient>(eventType, implementationType);
                
                _delegateCache.TryAdd(key, lambda);
            }
        }

        foreach (var (eventType, eventData) in GetCommandEventData())
        {
            foreach (var (implementationType, _) in eventData.Subscribers)
            {
                var key = SubscriberDelegateKey.Create(implementationType, eventType);
                
                if (_delegateCache.ContainsKey(key))
                {
                    continue;
                }
                
                var lambda = CreateDelegate<CommandsExtension>(eventType, implementationType);
                
                _delegateCache.TryAdd(key, lambda);
            }
        }
    }

    private static SubscriberDelegate CreateDelegate<TSender>(Type eventType, Type implementationType)
    {
        var method = implementationType.GetMethods().First(x =>
            x.Name == nameof(IDiscordBasicEventSubscriber<DiscordClient, DiscordEventArgs>.OnEventAsync) &&
            x.IsAbstract == false &&
            x.GetParameters() is { Length: 2 } &&
            x.GetParameters()[0].ParameterType == typeof(TSender) &&
            x.GetParameters()[1].ParameterType == eventType && 
            x.ReturnType == typeof(Task));

        var sender = Expression.Parameter(typeof(object), "sender");
        var args = Expression.Parameter(typeof(AsyncEventArgs), "args");
        var suber = Expression.Parameter(typeof(object), "suber");

        var convertSender = Expression.Convert(sender, typeof(TSender));
        var convertArgs = Expression.Convert(args, eventType);
        var convertSuber = Expression.Convert(suber, implementationType);

        var call = Expression.Call(convertSuber, method, convertSender, convertArgs);

        var lambda = Expression.Lambda<SubscriberDelegate>(call, suber, sender, args);
        
        return lambda.Compile();
    }

    internal MetadataProvider AppendTypes(IEnumerable<Type> types)
    {
        var subscriberTypes = types.Where(x => x is { IsClass: true, IsAbstract: false } 
                                               && x.GetInterfaces().Any(y => y == typeof(IDiscordBasicEventSubscriber)));

        var subscriberMetadatas = subscriberTypes.Select(SubscriberMetadata.Create).ToArray();

        foreach (var metadata in subscriberMetadatas)
        {
            foreach (var eventType in metadata.EventTypes)
            {
                var eventTypeEnum = TypeHelper.GetEventType(eventType);
                
                var specificDictionary = eventTypeEnum switch
                {
                    EventType.Basic => _basicEventData,
                    EventType.Command => _commandEventData,
                    _ => throw new InvalidOperationException($"Invalid event type {eventType.Name}")
                };
                
                if (!_eventData.TryGetValue(eventType, out var eventMetadata))
                {
                    var newMetadata = DiscordEventMetadata.Create(eventType, eventTypeEnum,
                        new Dictionary<Type, SubscriberMetadata>() { { metadata.ImplementationType, metadata } });
                    
                    _eventData.TryAdd(eventType, newMetadata);
                    
                    specificDictionary.TryAdd(eventType, newMetadata);
                }
                else
                {
                    eventMetadata.Subscribers.TryAdd(metadata.ImplementationType, metadata);
                    specificDictionary[eventType].Subscribers.TryAdd(metadata.ImplementationType, metadata);
                }
            }
        }

        PrepareAndCacheDelegates();
        
        return this;
    }
    

    internal MetadataProvider AppendTypes(IEnumerable<Assembly>? assemblies = null)
    {
        var asm = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
        
        var asmTypes = asm.SelectMany(x => x.GetTypes());

        return AppendTypes(asmTypes);
    }
}
