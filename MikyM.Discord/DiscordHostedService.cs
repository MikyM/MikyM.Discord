// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 MikyM
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MikyM.Discord.Interfaces;
using MikyM.Discord.Services;
using OpenTracing;

namespace MikyM.Discord
{
    /// <summary>
    ///     Brings a <see cref="IDiscordService" /> online.
    /// </summary>
    [UsedImplicitly]
    public class DiscordHostedService : IHostedService
    {
        private readonly IDiscordService _discordClient;

        private readonly ILogger<DiscordHostedService> _logger;

        private readonly ITracer _tracer;

        public DiscordHostedService(
            IDiscordService discordClient,
            ITracer tracer,
            ILogger<DiscordHostedService> logger)
        {
            _discordClient = discordClient;
            _tracer = tracer;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ((DiscordService) _discordClient).Initialize();

            using (_tracer.BuildSpan(nameof(_discordClient.Client.ConnectAsync)).StartActive(true))
            {
                _logger.LogInformation("Connecting to Discord API...");
                await _discordClient.Client.ConnectAsync();
                _logger.LogInformation("Connected");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordClient.Client.DisconnectAsync();
        }
    }
}