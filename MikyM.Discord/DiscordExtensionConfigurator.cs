using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace MikyM.Discord;

/// <summary>
/// Represents the configuration for the Discord extensions.
/// </summary>
internal sealed class DiscordExtensionConfigurator<TExtension> : DiscordExtensionConfigurator where TExtension : BaseExtension
{
    private readonly Func<DiscordClient,IServiceProvider,Task> _extensionConfigureAction;
    
    /// <inheritdoc/>
    internal override string ExtensionName => typeof(TExtension).Name;

    /// <summary>
    /// Creates a new instance of <see cref="DiscordExtensionConfigurator{TInner}"/>.
    /// </summary>
    /// <param name="extensionConfigureAction">The action to configure given extension.</param>
    public DiscordExtensionConfigurator(Func<DiscordClient,IServiceProvider,Task> extensionConfigureAction)
    {
        _extensionConfigureAction = extensionConfigureAction;
    }
    
    /// <inheritdoc/>
    internal override async Task ConfigureAsync(DiscordClient client, IServiceProvider serviceProvider)
    {
        await _extensionConfigureAction(client, serviceProvider);
    }
}

/// <summary>
/// Represents the configuration for the Discord extensions.
/// </summary>
internal abstract class DiscordExtensionConfigurator
{
    /// <summary>
    /// Configures the extension.
    /// </summary>
    /// <param name="client">The client to configure the extension on.</param>
    /// <param name="serviceProvider">The service provider to use.</param>
    internal abstract Task ConfigureAsync(DiscordClient client, IServiceProvider serviceProvider);
    
    /// <summary>
    /// The name of the extension.
    /// </summary>
    internal abstract string ExtensionName { get; }
}
