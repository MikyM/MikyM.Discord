using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Commands;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MikyM.Discord;

/// <summary>
///     The executor for the Discord event subscribers.
/// </summary>
[PublicAPI]
[UsedImplicitly]
public class DiscordEventDispatcher : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    
    private readonly ILogger<DiscordEventDispatcher> _logger;

    private readonly MetadataProvider _metadataProvider;

    private readonly IOptions<DiscordEventDispatchConfiguration> _options;

    private bool _isEnabled;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordEventDispatcher"/>.
    /// </summary>
    /// <param name="serviceProvider">The root service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="metadataProvider">Metadata provider.</param>
    /// <param name="options">The options.</param>
    public DiscordEventDispatcher(IServiceProvider serviceProvider, ILogger<DiscordEventDispatcher> logger, MetadataProvider metadataProvider, 
        IOptions<DiscordEventDispatchConfiguration> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _metadataProvider = metadataProvider;
        _options = options;
    }

    /// <summary>
    /// Executes the event subscriber within it's own scope.
    /// </summary>
    /// <param name="implementation">The subscriber service type.</param>
    /// <param name="eventType">The event type.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event args.</param>
    public async Task DispatchSingleAsync(Type implementation, Type eventType, DiscordClient sender, DiscordEventArgs eventArgs)
    {
        _logger.LogDebug("Executing {@Sub} for {@EventType}", implementation.Name, eventType.Name);
        
        if (!_metadataProvider.GetBasicEventData().TryGetValue(eventType, out var eventMetadata))
        {
            return;
        }
        
        if (!eventMetadata.Subscribers.TryGetValue(implementation, out var metadata))
        {
            _logger.LogError("Subscriber {Subscriber} not found for {EventType}", implementation.Name, eventType.Name);
            
            return;
        }
        
        await using var scope = _serviceProvider.CreateAsyncScope();

        var subscriber = metadata.ResolveStrategy switch
        {
            ResolveStrategy.Implementation => (IDiscordBasicEventSubscriber)scope.ServiceProvider.GetRequiredService(
                metadata.ImplementationType),
            ResolveStrategy.KeyedInterface => (IDiscordBasicEventSubscriber)
                scope.ServiceProvider.GetRequiredKeyedService(
                    metadata.ImplementedInterfacesInfo[eventType].InterfaceType, metadata.ImplementationType.Name),
            _ => throw new ArgumentOutOfRangeException()
        };

        var func = _metadataProvider.GetSubscriberDelegate(metadata.ImplementationType, eventType);

        await func(subscriber, sender, eventArgs);
    }
    
    /// <summary>
    /// Executes the event subscriber within it's own scope.
    /// </summary>
    /// <param name="implementation">The subscriber service type.</param>
    /// <param name="eventType">The event type.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event args.</param>
    public async Task DispatchSingleAsync(Type implementation, Type eventType, CommandsExtension sender, AsyncEventArgs eventArgs)
    {
        _logger.LogDebug("Executing {@Sub} for {@EventType}", implementation.Name, eventType.Name);
        
        if (!_metadataProvider.GetCommandEventData().TryGetValue(eventType, out var eventMetadata))
        {
            return;
        }
        
        if (!eventMetadata.Subscribers.TryGetValue(implementation, out var metadata))
        {
            _logger.LogError("Subscriber {Subscriber} not found for {EventType}", implementation.Name, eventType.Name);

            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();

        var subscriber = metadata.ResolveStrategy switch
        {
            ResolveStrategy.Implementation => scope.ServiceProvider
                .GetRequiredService(metadata.ImplementationType),
            ResolveStrategy.KeyedInterface => scope.ServiceProvider.GetRequiredKeyedService(
                    metadata.ImplementedInterfacesInfo[eventType].InterfaceType, metadata.ImplementationType.Name),
            _ => throw new ArgumentOutOfRangeException()
        };

        var func = _metadataProvider.GetSubscriberDelegate(metadata.ImplementationType, eventType);

        await func(subscriber, sender, eventArgs);
    }
    
    private static string GetDispatchId() => Guid.NewGuid().ToString();
    
    /// <summary>
    /// Executes the event subscriber pipe with scope per subscriber in parallel.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event args.</param>
    public async Task DispatchAsync(Type eventType, object sender, AsyncEventArgs eventArgs)
    {
        if (!_isEnabled)
        {
            return;
        }
        
        var dispatchId = GetDispatchId();
        
        _logger.LogDebug("Executing pipe for {@EventType} and args {@Args}, dispatch {@Dispatch}", eventType.Name, eventArgs.GetHashCode(), dispatchId);

        if (!_metadataProvider.GetEventData().TryGetValue(eventType, out var eventMetadata))
        {
            _logger.LogDebug("No subscribers found for {EventType}, dispatch {@Dispatch}", eventType.Name, dispatchId);
            
            return;
        }
        
        _logger.LogDebug("Found {Count} subscribers for {EventType}, dispatch {@Dispatch}", eventMetadata.Subscribers.Count, eventType.Name, dispatchId);
        
        _logger.LogDebug("Starting dispatch {@Dispatch} for {EventName}", dispatchId, eventType.Name);
        
        var strategy = eventMetadata.EventType switch
        {
            EventType.Basic => _options.Value.BasicDispatchStrategy,
            EventType.Command => _options.Value.CommandDispatchStrategy,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var parallelism = eventMetadata.EventType switch
        {
            EventType.Basic => _options.Value.MaxDegreeOfBasicEventParallelism,
            EventType.Command => _options.Value.MaxDegreeOfCommandEventParallelism,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var dispatchScope = eventMetadata.EventType switch
        {
            EventType.Basic => _options.Value.BasicDispatchScope,
            EventType.Command => _options.Value.CommandDispatchScope,
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (strategy)
        {
            case DispatchStrategy.Sequential:
            {
                switch (dispatchScope)
                {
                    case DispatchScope.PerSubscriber:
                    {
                        var maps = eventMetadata.Subscribers.Select(x =>
                        {
                            var scope = _serviceProvider.CreateAsyncScope();

                            var subscriber = x.Value.ResolveStrategy switch
                            {
                                ResolveStrategy.Implementation => scope.ServiceProvider
                                    .GetRequiredService(x.Value.ImplementationType),
                                ResolveStrategy.KeyedInterface => scope.ServiceProvider.GetRequiredKeyedService(
                                    x.Value.ImplementedInterfacesInfo[eventType].InterfaceType,
                                    x.Value.ImplementationType.Name),
                                _ => throw new ArgumentOutOfRangeException()
                            };

                            return new
                            {
                                Scope = scope,
                                Subscriber = subscriber,
                                Order = x.Value.Order
                            };
                        }).ToArray();

                        try
                        {
                            foreach (var map in maps.OrderBy(x => x.Order))
                            {
                                var func = _metadataProvider.GetSubscriberDelegate(map.Subscriber.GetType(),
                                    eventType);

                                await func(map.Subscriber, sender, eventArgs);
                            }
                        }
                        finally
                        {
                            foreach (var map in maps)
                            {
                                await map.Scope.DisposeAsync();
                            }
                        }
                
                        break;
                    }
                    case DispatchScope.PerEvent:
                    {
                        await using var scope = _serviceProvider.CreateAsyncScope();
                        
                        var maps = eventMetadata.Subscribers.Select(x =>
                        {
                            var subscriber = x.Value.ResolveStrategy switch
                            {
                                ResolveStrategy.Implementation => scope.ServiceProvider
                                    .GetRequiredService(x.Value.ImplementationType),
                                ResolveStrategy.KeyedInterface => scope.ServiceProvider.GetRequiredKeyedService(
                                    x.Value.ImplementedInterfacesInfo[eventType].InterfaceType,
                                    x.Value.ImplementationType.Name),
                                _ => throw new ArgumentOutOfRangeException()
                            };

                            return new
                            {
                                Subscriber = subscriber,
                                Order = x.Value.Order
                            };
                        }).ToArray();

                        foreach (var map in maps.OrderBy(x => x.Order))
                        {
                            var func = _metadataProvider.GetSubscriberDelegate(map.Subscriber.GetType(),
                                eventType);

                            await func(map.Subscriber, sender, eventArgs);
                        }
                        
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                
                break;
            }
            case DispatchStrategy.Parallel:
            {
                switch (dispatchScope)
                {
                    case DispatchScope.PerSubscriber:
                    {
                        await Parallel.ForEachAsync(eventMetadata.Subscribers,
                            new ParallelOptions() { MaxDegreeOfParallelism = parallelism ?? -1 },
                            async (metadata, _) =>
                            {
                                await using var scope = _serviceProvider.CreateAsyncScope();

                                var subscriber = metadata.Value.ResolveStrategy switch
                                {
                                    ResolveStrategy.Implementation => scope.ServiceProvider.GetRequiredService(metadata.Value
                                        .ImplementationType),
                                    ResolveStrategy.KeyedInterface => scope.ServiceProvider.GetRequiredKeyedService(
                                        metadata.Value.ImplementedInterfacesInfo[eventType].InterfaceType,
                                        metadata.Value.ImplementationType.Name),
                                    _ => throw new ArgumentOutOfRangeException()
                                };

                                var func = _metadataProvider.GetSubscriberDelegate(metadata.Value.ImplementationType,
                                    eventType);

                                await func(subscriber, sender, eventArgs);
                            });
                        
                        break;
                    }
                    case DispatchScope.PerEvent:
                    {
                        await using var scope = _serviceProvider.CreateAsyncScope();
                        
                        var maps = eventMetadata.Subscribers.Select(x =>
                        {
                            var subscriber = x.Value.ResolveStrategy switch
                            {
                                ResolveStrategy.Implementation => scope.ServiceProvider
                                    .GetRequiredService(x.Value.ImplementationType),
                                ResolveStrategy.KeyedInterface => scope.ServiceProvider.GetRequiredKeyedService(
                                    x.Value.ImplementedInterfacesInfo[eventType].InterfaceType,
                                    x.Value.ImplementationType.Name),
                                _ => throw new ArgumentOutOfRangeException()
                            };

                            return new
                            {
                                Subscriber = subscriber,
                                Metadata = x.Value
                            };
                        }).ToArray();
                        
                        await Parallel.ForEachAsync(maps,
                            new ParallelOptions() { MaxDegreeOfParallelism = parallelism ?? -1 },
                            async (map, _) =>
                            {
                                var func = _metadataProvider.GetSubscriberDelegate(map.Metadata.ImplementationType,
                                    eventType);

                                await func(map.Subscriber, sender, eventArgs);
                            });
                        
                        break;
                    }
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }

                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        
        _logger.LogDebug("Finished dispatch {@Dispatch} for {EventType}", dispatchId, eventType.Name);
        
        _logger.LogDebug("Pipe for {@EventType} and args {@Args}, dispatch {@Dispatch} finished", eventType.Name, eventArgs.GetHashCode(), dispatchId);
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting discord event dispatch to subscribers..");
        
        _isEnabled = true;
        
        _logger.LogDebug("Discord event dispatch to subscribers started");
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Stopping discord event dispatch to subscribers..");
        
        _isEnabled = false;
        
        _logger.LogDebug("Discord event dispatch to subscribers stoppped");
        
        return Task.CompletedTask;
    }
}
