// MIT License
//
// Copyright (c) 2021 Benjamin Höglinger-Stelzer
// Copyright (c) 2021 Krzysztof Kupisz - MikyM
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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