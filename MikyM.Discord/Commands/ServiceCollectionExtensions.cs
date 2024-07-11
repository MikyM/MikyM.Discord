using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using Microsoft.Extensions.DependencyInjection;

namespace MikyM.Discord.Commands;

/// <summary>
/// Service collection extensions for Discord commands.
/// </summary>
[UsedImplicitly]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the Commands extension to the <see cref="DiscordClient" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="assembliesToScan">Assemblies to scan.</param>
    /// <param name="configuration">The <see cref="CommandsConfiguration" />.</param>
    /// <param name="extension">
    ///     The <see cref="CommandsExtension" />.
    /// </param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDiscordCommands(
        this IServiceCollection services,
        IEnumerable<Assembly> assembliesToScan,
        Action<CommandsConfiguration>? configuration = null,
        Action<CommandsExtension>? extension = null
    )
    {
        var metadataProvider = MetadataProvider.Instance;
        
        metadataProvider.AppendTypes(assembliesToScan);

        var subscriberTypes = metadataProvider.GetCommandEventData();
        
        var options = new CommandsConfiguration();

        configuration?.Invoke(options);
        
        foreach (var (_, metas) in subscriberTypes)
        {
            foreach (var (_,metadata) in metas.Subscribers)
            {
                services.AddDiscordEventSubscriber(metadata);
            }
        }
        
        services.AddExtensionConfigurator<CommandsExtension>(Configure);

        return services;

        Task Configure(DiscordClient client, IServiceProvider provider)
        {
            var ext = client.UseCommands(options);

            extension?.Invoke(ext);

            var executor = provider.GetRequiredService<DiscordEventDispatcher>();

            ext.CommandErrored += (sender, args) =>
            {
                _ = executor.DispatchSequentialOrderedPipeAsync(typeof(CommandErroredEventArgs), sender, args);

                return Task.CompletedTask;
            };

            ext.CommandExecuted += (sender, args) =>
            {
                _ = executor.DispatchSequentialOrderedPipeAsync(typeof(CommandExecutedEventArgs), sender, args);

                return Task.CompletedTask;
            };
            
            return Task.CompletedTask;
        }
    }
}
