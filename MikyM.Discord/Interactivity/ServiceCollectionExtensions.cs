using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MikyM.Discord.Interactivity;

/// <summary>
/// The service collection extensions for Discord Interactivity.
/// </summary>
[UsedImplicitly]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Interactivity extension to <see cref="DiscordClient" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configuration">The <see cref="InteractivityConfiguration" />.</param>
    /// <param name="extension">The <see cref="InteractivityExtension" /></param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDiscordInteractivity(
        this IServiceCollection services,
        Action<InteractivityConfiguration> configuration,
        Action<InteractivityExtension>? extension = null
    )
    {
        var options = new InteractivityConfiguration();

        configuration(options);
        
        services.AddExtensionConfigurator<InteractivityExtension>(Configure);

        return services;

        Task Configure(DiscordClient client, IServiceProvider provider)
        {
            var ext = client.UseInteractivity(options);

            extension?.Invoke(ext);
            
            return Task.CompletedTask;
        }
    }
}
