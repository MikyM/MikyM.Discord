using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MikyM.Discord;

/// <summary>
/// The discord service.
/// </summary>
[UsedImplicitly]
internal sealed class DiscordService : IHostedService
{
    private readonly DiscordClient _discordClient;
    private readonly ILogger<DiscordService> _logger;
    private readonly IEnumerable<DiscordExtensionConfigurator> _extensionConfigurators;
    private readonly IServiceProvider _rootProvider;
    
    public DiscordService(DiscordClient discordClient, ILogger<DiscordService> logger, 
        IEnumerable<DiscordExtensionConfigurator> extensionConfigurators, IServiceProvider rootProvider)
    {
        _discordClient = discordClient;
        _logger = logger;
        _extensionConfigurators = extensionConfigurators;
        _rootProvider = rootProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ext = _extensionConfigurators.ToArray();

        if (ext.Length != 0)
        {
            _logger.LogInformation("Configuring {Length} Discord extensions..", ext.Length);
        
            foreach (var configurator in ext)
            {
                _logger.LogInformation("Configuring {Extension}..", configurator.ExtensionName);
                
                await configurator.ConfigureAsync(_discordClient, _rootProvider);
                
                _logger.LogInformation("Configured {Extension}", configurator.ExtensionName);
            }
        }
        
        _logger.LogInformation("Connecting to Discord..");
        
        await _discordClient.ConnectAsync();
        
        _logger.LogInformation("Connected to Discord");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting from Discord..");
        
        await _discordClient.DisconnectAsync();
        
        _logger.LogInformation("Disconnected from Discord");
    }
}
