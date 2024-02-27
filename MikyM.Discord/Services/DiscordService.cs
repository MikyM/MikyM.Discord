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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using DSharpPlus;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MikyM.Common.Utilities;
using MikyM.Discord.Events;
using MikyM.Discord.Interfaces;
using MikyM.Discord.Util;
using OpenTracing;

namespace MikyM.Discord.Services;

/// <summary>
///     An implementation of <see cref="IDiscordService" />.
/// </summary>
[UsedImplicitly]
public class DiscordService : IDiscordService
{
    private readonly IOptions<DiscordConfiguration> _discordOptions;
    private readonly ILoggerFactory _logFactory;
    private readonly ILogger<DiscordService> _logger;
    private readonly ILifetimeScope _rootScope;
    private readonly ITracer _tracer;
    private readonly IBackgroundExecutor _asyncExecutor;

    private readonly IEnumerable<Type> _miscSubscribers;
    private readonly IEnumerable<Type> _voiceSubscribers;
    private readonly IEnumerable<Type> _presenceSubscribers;
    private readonly IEnumerable<Type> _reactionSubscribers;
    private readonly IEnumerable<Type> _messageSubscribers;
    private readonly IEnumerable<Type> _inviteSubscribers;
    private readonly IEnumerable<Type> _roleSubscribers;
    private readonly IEnumerable<Type> _memberSubscribers;
    private readonly IEnumerable<Type> _banSubscribers;
    private readonly IEnumerable<Type> _guildSubscribers;
    private readonly IEnumerable<Type> _channelSubscribers;
    private readonly IEnumerable<Type> _websocketSubscribers;

    public DiscordService(ILoggerFactory logFactory, ILogger<DiscordService> logger,
        ITracer tracer, IOptions<DiscordConfiguration> discordOptions, IBackgroundExecutor asyncExecutor, ILifetimeScope rootScope)
    {
        _logger = logger;
        _tracer = tracer;
        _discordOptions = discordOptions;
        _asyncExecutor = asyncExecutor;
        _rootScope = rootScope;
        _logFactory = logFactory;

        _miscSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordMiscEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _voiceSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordVoiceEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _presenceSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordPresenceUserEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _reactionSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordMessageReactionEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _messageSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordMessageEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _inviteSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordInviteEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _roleSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordGuildRoleEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _memberSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordGuildMemberEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _banSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordGuildBanEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _guildSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordGuildEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _channelSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordChannelEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
        _websocketSubscribers = _rootScope.ComponentRegistry.Registrations
            .Where(r => typeof(IDiscordWebSocketEventsSubscriber).IsAssignableFrom(r.Activator.LimitType))
            .Select(r => r.Activator.LimitType)
            .Distinct();
    }

    public DiscordClient Client { get; private set; } = null!;

    internal void Initialize()
    {
        if (_discordOptions.Value  is null)
            throw new InvalidOperationException($"{nameof(DiscordConfiguration)} option is required");

        using var serviceScope = _rootScope.BeginLifetimeScope();

        #region Subscriber services

        var channelEventsSubscriber = serviceScope.GetDiscordChannelEventsSubscribers();

        var guildEventSubscribers = serviceScope.GetDiscordGuildEventsSubscribers();

        var guildBanEventsSubscriber = serviceScope.GetDiscordGuildBanEventsSubscribers();

        var guildMemberEventsSubscriber = serviceScope.GetDiscordGuildMemberEventsSubscribers();

        var guildRoleEventsSubscriber = serviceScope.GetDiscordGuildRoleEventsSubscribers();

        var inviteEventsSubscriber = serviceScope.GetDiscordInviteEventsSubscribers();

        var messageEventsSubscriber = serviceScope.GetDiscordMessageEventsSubscribers();

        var messageReactionAddedEventsSubscriber = serviceScope.GetDiscordMessageReactionAddedEventsSubscribers();

        var presenceUserEventsSubscriber = serviceScope.GetDiscordPresenceUserEventsSubscribers();

        var voiceEventsSubscriber = serviceScope.GetDiscordVoiceEventsSubscribers();

        #endregion

        #region Build intents

        //
        // Grab the content of the user-set intents and merge them with what the subscribers need
        //
        var property = typeof(DiscordConfiguration).GetProperty("Intents");
        property = property?.DeclaringType?.GetProperty("Intents");
        var intents = (DiscordIntents) (property?.GetValue(_discordOptions.Value,
            BindingFlags.NonPublic | BindingFlags.Instance, null, null, null) ?? throw new InvalidOperationException());

        //
        // Merge/enrich intents the user requested with those the subscribers require
        //

#if NET7_0
        if (channelEventsSubscriber.Any()) intents |= DiscordIntents.Guilds;
        if (guildEventSubscribers.Any()) intents |= DiscordIntents.Guilds | DiscordIntents.GuildEmojis;
        if (guildBanEventsSubscriber.Any()) intents |= DiscordIntents.GuildBans;
        if (guildMemberEventsSubscriber.Any()) intents |= DiscordIntents.GuildMembers;
        if (guildRoleEventsSubscriber.Any()) intents |= DiscordIntents.Guilds;
        if (inviteEventsSubscriber.Any()) intents |= DiscordIntents.GuildInvites;
        if (messageEventsSubscriber.Any()) intents |= DiscordIntents.GuildMessages;
        if (messageReactionAddedEventsSubscriber.Any()) intents |= DiscordIntents.GuildMessageReactions;
        if (presenceUserEventsSubscriber.Any()) intents |= DiscordIntents.GuildPresences;
        if (voiceEventsSubscriber.Any()) intents |= DiscordIntents.GuildVoiceStates;
#else
        if (channelEventsSubscriber.Any()) intents |= DiscordIntents.Guilds;
        if (guildEventSubscribers.Any()) intents |= DiscordIntents.Guilds | DiscordIntents.GuildEmojisAndStickers;
        if (guildBanEventsSubscriber.Any()) intents |= DiscordIntents.GuildModeration;
        if (guildMemberEventsSubscriber.Any()) intents |= DiscordIntents.GuildMembers;
        if (guildRoleEventsSubscriber.Any()) intents |= DiscordIntents.Guilds;
        if (inviteEventsSubscriber.Any()) intents |= DiscordIntents.GuildInvites;
        if (messageEventsSubscriber.Any()) intents |= DiscordIntents.GuildMessages;
        if (messageReactionAddedEventsSubscriber.Any()) intents |= DiscordIntents.GuildMessageReactions;
        if (presenceUserEventsSubscriber.Any()) intents |= DiscordIntents.GuildPresences;
        if (voiceEventsSubscriber.Any()) intents |= DiscordIntents.GuildVoiceStates;
#endif


        #endregion

        var configuration = new DiscordConfiguration(_discordOptions.Value)
        {
            //
            // Overwrite with DI configured logging factory
            //
            LoggerFactory = _logFactory,
            //
            // Use merged intents
            //
            Intents = intents
        };

        Client = new DiscordClient(configuration);

        //
        // Load options that should load in before Connect call
        //
        _rootScope.Resolve<IEnumerable<IDiscordExtensionConfiguration>>();

        #region WebSocket

        Client.SocketErrored += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.SocketErrored))
                .IgnoreActiveSpan()
                .StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnSocketErrored(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.SocketOpened += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.SocketOpened))
                .IgnoreActiveSpan()
                .StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnSocketOpened(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.SocketClosed += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.SocketClosed))
                .IgnoreActiveSpan()
                .StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnSocketClosed(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

#if NET7_0
           Client.Ready += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.Ready)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnReady(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.Resumed += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.Resumed)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnResumed(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };     
#else
        Client.GuildDownloadCompleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildDownloadCompleted)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildDownloadCompleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };
        
        Client.SessionCreated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.SessionCreated)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnReady(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.SessionResumed += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.SessionResumed)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnResumed(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };
#endif
        


        Client.Heartbeated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.Heartbeated)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnHeartbeated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.Zombied += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.Zombied)).IgnoreActiveSpan().StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_websocketSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordWebSocketEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnZombied(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Channel

        Client.ChannelCreated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.ChannelCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_channelSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordChannelEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnChannelCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.ChannelUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.ChannelUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("ChannelBefore.Id", args.ChannelBefore.Id.ToString());
            workScope.Span.SetTag("ChannelAfter.Id", args.ChannelAfter.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_channelSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordChannelEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnChannelUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.ChannelDeleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.ChannelDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_channelSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordChannelEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnChannelDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.DmChannelDeleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.DmChannelDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_channelSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordChannelEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnDmChannelDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.ChannelPinsUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.ChannelPinsUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_channelSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordChannelEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnChannelPinsUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Guild

        Client.GuildCreated += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.GuildCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildAvailable += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildAvailable))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildAvailable(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildUpdated += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.GuildUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);

            workScope.Span.SetTag("Guild.Id", args.GuildBefore.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildDeleted += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.GuildDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildUnavailable += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildUnavailable))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildUnavailable(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildDownloadCompleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildDownloadCompleted))
                .IgnoreActiveSpan()
                .StartActive(true);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildDownloadCompleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildEmojisUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildEmojisUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildEmojisUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildStickersUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildStickersUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildStickersUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildIntegrationsUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildIntegrationsUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_guildSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildIntegrationsUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Guild Ban

        Client.GuildBanAdded += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.GuildBanAdded))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_banSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildBanEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildBanAdded(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildBanRemoved += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildBanRemoved))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_banSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildBanEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildBanRemoved(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Guild Member

        Client.GuildMemberAdded += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildMemberAdded))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_memberSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildMemberEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildMemberAdded(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildMemberRemoved += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildMemberRemoved))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_memberSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildMemberEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildMemberRemoved(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildMemberUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildMemberUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_memberSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildMemberEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildMemberUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildMembersChunked += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildMembersChunked))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_memberSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildMemberEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildMembersChunked(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Guild Role

        Client.GuildRoleCreated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildRoleCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Role.Id", args.Role.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_roleSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildRoleEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildRoleCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildRoleUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildRoleUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("RoleBefore.Id", args.RoleBefore.Id.ToString());
            workScope.Span.SetTag("RoleAfter.Id", args.RoleAfter.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_roleSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildRoleEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildRoleUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.GuildRoleDeleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.GuildRoleDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Role.Id", args.Role.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_roleSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordGuildRoleEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnGuildRoleDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Invite

        Client.InviteCreated += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.InviteCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("Invite.Code", args.Invite.Code.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_inviteSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordInviteEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnInviteCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.InviteDeleted += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.InviteDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("Invite.Code", args.Invite.Code.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_inviteSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordInviteEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnInviteDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region message

        Client.MessageCreated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            if (args.Guild is not null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("Author.Id", args.Author.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_messageSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };
#if NET7_0
           Client.MessageAcknowledged += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageAcknowledged))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_messageSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageAcknowledged(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };     
#endif


        Client.MessageUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            if (args.Guild is not null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            if (args.Author is not null) workScope.Span.SetTag("Author.Id", args.Author.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_messageSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.MessageDeleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            if (args.Guild is not null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_messageSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.MessagesBulkDeleted += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessagesBulkDeleted))
                .IgnoreActiveSpan()
                .StartActive(true);
            if (args.Guild is not null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_messageSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessagesBulkDeleted(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region message Reaction

        Client.MessageReactionAdded += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageReactionAdded))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id);
            workScope.Span.SetTag("Channel.Id", args.Channel.Id);
            workScope.Span.SetTag("User.Id", args.User.Id);
            workScope.Span.SetTag("message.Id", args.Message.Id);

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_reactionSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageReactionEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageReactionAdded(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.MessageReactionRemoved += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageReactionRemoved))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("User.Id", args.User.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_reactionSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageReactionEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageReactionRemoved(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.MessageReactionsCleared += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageReactionsCleared))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_reactionSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageReactionEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageReactionsCleared(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.MessageReactionRemovedEmoji += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.MessageReactionRemovedEmoji))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("message.Id", args.Message.Id.ToString());
            workScope.Span.SetTag("Emoji.Id", args.Emoji.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_reactionSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMessageReactionEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnMessageReactionRemovedEmoji(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Presence/User Update

        Client.PresenceUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.PresenceUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("User.Id", args.User?.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_presenceSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordPresenceUserEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnPresenceUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.UserSettingsUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.UserSettingsUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("User.Id", args.User?.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_presenceSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordPresenceUserEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnUserSettingsUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.UserUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.UserUpdated)).IgnoreActiveSpan().StartActive(true);
            workScope.Span.SetTag("UserBefore.Id", args.UserBefore?.Id.ToString());
            workScope.Span.SetTag("UserBefore.Id", args.UserBefore?.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_presenceSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordPresenceUserEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnUserUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Voice

        Client.VoiceStateUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.VoiceStateUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            if (args.Channel is not null) workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("User.Id", args.User.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_voiceSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordVoiceEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnVoiceStateUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.VoiceServerUpdated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.VoiceServerUpdated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_voiceSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordVoiceEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnVoiceServerUpdated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion

        #region Misc

        Client.ComponentInteractionCreated += (sender, args) =>
        {
            using var workScope = _tracer.BuildSpan(nameof(Client.ComponentInteractionCreated))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
            workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
            workScope.Span.SetTag("User.Id", args.User.Id.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_miscSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMiscEventsSubscriber) scope.Resolve(sub);
                        await handler.DiscordOnComponentInteractionCreated(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        Client.ClientErrored += (sender, args) =>
        {
            using var workScope = _tracer
                .BuildSpan(nameof(Client.ClientErrored))
                .IgnoreActiveSpan()
                .StartActive(true);
            workScope.Span.SetTag("ClientErrored", args.EventName.ToString());

            _asyncExecutor.ExecuteAsync(async () =>
            {
                await Parallel.ForEachAsync(_miscSubscribers, async (sub, _) =>
                {
                    await using var scope = _rootScope.BeginLifetimeScope();
                    {
                        var handler = (IDiscordMiscEventsSubscriber)scope.Resolve(sub);
                        await handler.DiscordOnClientErrored(sender, args);
                    }
                });
            });

            return Task.CompletedTask;
        };

        #endregion
    }
}
