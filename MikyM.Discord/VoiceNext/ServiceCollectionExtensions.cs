using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;

namespace MikyM.Discord.VoiceNext;

/// <summary>
/// Service collection extensions for Discord VoiceNext.
/// </summary>
[UsedImplicitly]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds VoiceNext extension to <see cref="DiscordClient" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configure">The <see cref="VoiceNextConfiguration" />.</param>
    /// <param name="extension">The extension action.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDiscordVoiceNext(
        this IServiceCollection services,
        Action<VoiceNextConfiguration>? configure = null,
        Action<VoiceNextExtension>? extension = null
    )
    {
        var options = new VoiceNextConfiguration();

        configure?.Invoke(options);
        
        services.AddExtensionConfigurator<VoiceNextExtension>(Configure);

        return services;
        
        Task Configure(DiscordClient client, IServiceProvider provider)
        {
            var ext = client.UseVoiceNext(options);

            extension?.Invoke(ext);
            
            return Task.CompletedTask;
        }
    }
}
