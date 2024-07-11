using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.EventArgs;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MikyM.Discord;

/// <summary>
/// Extensions for <see cref="IServiceCollection" />.
/// </summary>
[PublicAPI]
[UsedImplicitly]
public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddExtensionConfigurator<TExtension>(this IServiceCollection services, Func<DiscordClient,IServiceProvider,Task> configuringAction)
        where TExtension : BaseExtension
    {
        services.AddSingleton(new DiscordExtensionConfigurator<TExtension>(configuringAction));
        
        services.AddSingleton<DiscordExtensionConfigurator>(x =>
            x.GetRequiredService<DiscordExtensionConfigurator<TExtension>>());
        
        return services;
    }
    
    /// <summary>
    /// Adds the given subscriber to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementation">Implementation type.</param>
    /// <returns>The service collection.</returns>
    internal static IServiceCollection AddDiscordEventSubscriber(this IServiceCollection services, Type implementation)
        => AddDiscordEventSubscriber(services, SubscriberMetadata.Create(implementation));
    
    /// <summary>
    /// Adds the given subscriber to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    internal static IServiceCollection AddDiscordEventSubscriber<TSubscriber>(this IServiceCollection services)
        => AddDiscordEventSubscriber(services, SubscriberMetadata.Create(typeof(TSubscriber)));

    /// <summary>
    /// Adds the given subscriber to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementation">Implementation type.</param>
    /// <returns>The service collection.</returns>
    internal static IServiceCollection AddDiscordEventSubscriber(this IServiceCollection services, Type serviceType, Type implementation)
        => AddDiscordEventSubscriber(services, SubscriberMetadata.Create(serviceType, implementation));
    
    /// <summary>
    /// Adds the given subscriber to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    internal static IServiceCollection AddDiscordEventSubscriber<TSubscriberService,TSubscriberImplementation>(this IServiceCollection services)
        => AddDiscordEventSubscriber(services, SubscriberMetadata.Create(typeof(TSubscriberImplementation),typeof(TSubscriberService)));
    
    internal static IServiceCollection AddDiscordEventSubscriber(this IServiceCollection services, SubscriberMetadata metadata, SubscriberType? subscriberType = null)
    {
        services.TryAddScoped(metadata.ImplementationType);

        services.TryAddKeyedScoped(typeof(IDiscordBasicEventSubscriber), metadata.ImplementationType.Name,
            (x,_) => x.GetRequiredService(metadata.ImplementationType));
        
        var infos = subscriberType is null 
            ? metadata.ImplementedInterfacesInfo 
            : metadata.ImplementedInterfacesInfo.Where(x => x.Value.Type == subscriberType);
        
        foreach (var (eventType, info) in infos)
        {
            services.TryAddScoped(info.InterfaceType, x => x.GetRequiredService(metadata.ImplementationType));
                
            services.TryAddKeyedScoped(info.InterfaceType, metadata.ImplementationType.Name, (x,_) => x.GetRequiredService(metadata.ImplementationType));
            
            services.TryAddKeyedScoped(typeof(IDiscordBasicEventSubscriber), eventType.Name,(x,_) => x.GetRequiredService(metadata.ImplementationType));
        }

        return services;
    }
    
    /// <summary>
    ///     Registers the Discord related services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="assembliesToScan">Assemblies to scan for subscribers.</param>
    /// <param name="token">The bot token.</param>
    /// <param name="intents">The intents.</param>
    /// <param name="dispatchConfigure">The dispatch configure.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddExtendedDiscord(
        this IServiceCollection services,
        string token,
        DiscordIntents intents,
        IEnumerable<Assembly> assembliesToScan,
        Action<DiscordEventDispatchConfiguration> dispatchConfigure
    )
        => AddExtendedDiscordPrivate(services, token, intents, assembliesToScan, null, dispatchConfigure);
    
    /// <summary>
    ///     Registers the Discord related services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="assembliesToScan">Assemblies to scan for subscribers.</param>
    /// <param name="token">The bot token.</param>
    /// <param name="intents">The intents.</param>
    /// <param name="configure">The <see cref="DiscordConfiguration" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddExtendedDiscord(
        this IServiceCollection services,
        string token,
        DiscordIntents intents,
        IEnumerable<Assembly> assembliesToScan,
        Action<DiscordConfiguration> configure
    )
        => AddExtendedDiscordPrivate(services, token, intents, assembliesToScan, configure , null);
    
    /// <summary>
    ///     Registers the Discord related services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="assembliesToScan">Assemblies to scan for subscribers.</param>
    /// <param name="configure">The <see cref="DiscordConfiguration" />.</param>
    /// <param name="token">The bot token.</param>
    /// <param name="intents">The intents.</param>
    /// <param name="dispatchConfigure">The dispatch configure.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddExtendedDiscord(
        this IServiceCollection services,
        string token,
        DiscordIntents intents,
        IEnumerable<Assembly> assembliesToScan,
        Action<DiscordConfiguration> configure,
        Action<DiscordEventDispatchConfiguration> dispatchConfigure
    )
        => AddExtendedDiscordPrivate(services, token, intents, assembliesToScan, configure, dispatchConfigure);
    
    /// <summary>
    ///     Registers the Discord related services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="assembliesToScan">Assemblies to scan for subscribers.</param>
    /// <param name="token">The bot token.</param>
    /// <param name="intents">The intents.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddExtendedDiscord(
        this IServiceCollection services,
        string token,
        DiscordIntents intents,
        IEnumerable<Assembly> assembliesToScan
    )
        => AddExtendedDiscordPrivate(services, token, intents, assembliesToScan, null, null);
        
    private static IServiceCollection AddExtendedDiscordPrivate(
        this IServiceCollection services,
        string token,
        DiscordIntents intents,
        IEnumerable<Assembly> assembliesToScan,
        Action<DiscordConfiguration>? configure,
        Action<DiscordEventDispatchConfiguration>? dispatchConfigure
    )
    {
        services.AddOptions();

        if (configure != null)
        {
            services.Configure(configure);
        }
        
        if (dispatchConfigure != null)
        {
            services.Configure(dispatchConfigure);
        }
        else
        {
            services.Configure<DiscordEventDispatchConfiguration>(_ => { });
        }
        
        // add events

        var metadataProvider = MetadataProvider.Instance;
        
        metadataProvider.AppendTypes(assembliesToScan.SelectMany(x => x.GetTypes()));

        services.AddSingleton<DiscordEventDispatcher>(x =>
            new DiscordEventDispatcher(x, x.GetRequiredService<ILogger<DiscordEventDispatcher>>(), metadataProvider,
                x.GetRequiredService<IOptions<DiscordEventDispatchConfiguration>>()));

        services.AddHostedService(x => x.GetRequiredService<DiscordEventDispatcher>());

        var subscriberTypes = metadataProvider.GetBasicEventData();
        
        services.ConfigureEventHandlers(x =>
        {
        });
        
        foreach (var (eventType, metas) in subscriberTypes)
        {
            foreach (var (_, metadata) in metas.Subscribers)
            {
                services.AddDiscordEventSubscriber(metadata);
            }
            
            services.AddSingleton<IConfigureOptions<EventHandlerCollection>>(x =>
            {
                var executor = x.GetRequiredService<DiscordEventDispatcher>();

                AsyncEventHandler<DiscordClient, DiscordEventArgs> handler = (client, args) =>
                {
                    _ = executor.DispatchAsync(eventType, client, args);

                    return Task.CompletedTask;
                };

                var namedOptions = new ConfigureNamedOptions<EventHandlerCollection>(Options.DefaultName,
                    eventsHandlerCollection =>
                        eventsHandlerCollection.DelegateHandlers.GetOrAdd(eventType, _ => []).Add(handler));

                return namedOptions;
            });
        }
        
        services.AddDiscordClient(token, intents);

        services.AddHostedService<DiscordService>();
        
        return services;
    }
}
