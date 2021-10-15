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

using System;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MikyM.Discord.Interfaces;
using MikyM.Discord.Util;
using OpenTracing;

namespace MikyM.Discord.Services
{
    /// <summary>
    ///     An implementation of <see cref="IDiscordService" />.
    /// </summary>
    [UsedImplicitly]
    public class DiscordService : IDiscordService
    {
        protected readonly IOptions<DiscordConfiguration> DiscordOptions;
        protected readonly ILoggerFactory LogFactory;

        protected readonly ILogger<DiscordService> Logger;

        protected readonly IServiceProvider ServiceProvider;

        protected readonly ITracer Tracer;

        public DiscordService(IServiceProvider serviceProvider, ILoggerFactory logFactory,
            ILogger<DiscordService> logger, ITracer tracer, IOptions<DiscordConfiguration> discordOptions)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
            Tracer = tracer;
            DiscordOptions = discordOptions;
            LogFactory = logFactory;
        }

        public DiscordClient Client { get; private set; }

        internal void Initialize()
        {
            if (DiscordOptions.Value is null)
                throw new InvalidOperationException($"{nameof(DiscordConfiguration)} option is required");

            using var serviceScope = ServiceProvider.CreateScope();

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
            property = property.DeclaringType.GetProperty("Intents");
            var intents = (DiscordIntents) property.GetValue(DiscordOptions.Value,
                BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);

            //
            // Merge/enrich intents the user requested with those the subscribers require
            // 

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

            #endregion

            var configuration = new DiscordConfiguration(DiscordOptions.Value)
            {
                //
                // Overwrite with DI configured logging factory
                // 
                LoggerFactory = LogFactory,
                //
                // Use merged intents
                // 
                Intents = intents
            };

            Client = new DiscordClient(configuration);

            //
            // Load options that should load in before Connect call
            // 
            ServiceProvider.GetServices<IDiscordExtensionConfiguration>();

            #region WebSocket

            Client.SocketErrored += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.SocketErrored))
                    .IgnoreActiveSpan()
                    .StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnSocketErrored(sender, args);
            };

            Client.SocketOpened += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.SocketOpened))
                    .IgnoreActiveSpan()
                    .StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnSocketOpened(sender, args);
            };

            Client.SocketClosed += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.SocketClosed))
                    .IgnoreActiveSpan()
                    .StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnSocketClosed(sender, args);
            };

            Client.Ready += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.Ready)).IgnoreActiveSpan().StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnReady(sender, args);
            };

            Client.Resumed += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.Resumed)).IgnoreActiveSpan().StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnResumed(sender, args);
            };

            Client.Heartbeated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.Heartbeated)).IgnoreActiveSpan().StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnHeartbeated(sender, args);
            };

            Client.Zombied += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.Zombied)).IgnoreActiveSpan().StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordWebSocketEventSubscribers())
                    await eventSubscriber.DiscordOnZombied(sender, args);
            };

            #endregion

            #region Channel

            Client.ChannelCreated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.ChannelCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordChannelEventsSubscribers())
                    await eventSubscriber.DiscordOnChannelCreated(sender, args);
            };

            Client.ChannelUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.ChannelUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("ChannelBefore.Id", args.ChannelBefore.Id.ToString());
                workScope.Span.SetTag("ChannelAfter.Id", args.ChannelAfter.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordChannelEventsSubscribers())
                    await eventSubscriber.DiscordOnChannelUpdated(sender, args);
            };

            Client.ChannelDeleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.ChannelDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordChannelEventsSubscribers())
                    await eventSubscriber.DiscordOnChannelDeleted(sender, args);
            };

            Client.DmChannelDeleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.DmChannelDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordChannelEventsSubscribers())
                    await eventSubscriber.DiscordOnDmChannelDeleted(sender, args);
            };

            Client.ChannelPinsUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.ChannelPinsUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordChannelEventsSubscribers())
                    await eventSubscriber.DiscordOnChannelPinsUpdated(sender, args);
            };

            #endregion

            #region Guild

            Client.GuildCreated += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.GuildCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildCreated(sender, args);
            };

            Client.GuildAvailable += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildAvailable))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildAvailable(sender, args);
            };

            Client.GuildUpdated += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.GuildUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.GuildBefore.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildUpdated(sender, args);
            };

            Client.GuildDeleted += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.GuildDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildDeleted(sender, args);
            };

            Client.GuildUnavailable += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildUnavailable))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildUnavailable(sender, args);
            };

            Client.GuildDownloadCompleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildDownloadCompleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildDownloadCompleted(sender, args);
            };

            Client.GuildEmojisUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildEmojisUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildEmojisUpdated(sender, args);
            };

            Client.GuildStickersUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildStickersUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildStickersUpdated(sender, args);
            };

            Client.GuildIntegrationsUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildIntegrationsUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildIntegrationsUpdated(sender, args);
            };

            #endregion

            #region Guild Ban

            Client.GuildBanAdded += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.GuildBanAdded))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildBanEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildBanAdded(sender, args);
            };

            Client.GuildBanRemoved += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildBanRemoved))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildBanEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildBanRemoved(sender, args);
            };

            #endregion

            #region Guild Member

            Client.GuildMemberAdded += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildMemberAdded))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildMemberEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildMemberAdded(sender, args);
            };

            Client.GuildMemberRemoved += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildMemberRemoved))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildMemberEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildMemberRemoved(sender, args);
            };

            Client.GuildMemberUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildMemberUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Member.Id", args.Member.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildMemberEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildMemberUpdated(sender, args);
            };

            Client.GuildMembersChunked += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildMembersChunked))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildMemberEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildMembersChunked(sender, args);
            };

            #endregion

            #region Guild Role

            Client.GuildRoleCreated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildRoleCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Role.Id", args.Role.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildRoleEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildRoleCreated(sender, args);
            };

            Client.GuildRoleUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildRoleUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("RoleBefore.Id", args.RoleBefore.Id.ToString());
                workScope.Span.SetTag("RoleAfter.Id", args.RoleAfter.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildRoleEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildRoleUpdated(sender, args);
            };

            Client.GuildRoleDeleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.GuildRoleDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Role.Id", args.Role.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordGuildRoleEventsSubscribers())
                    await eventSubscriber.DiscordOnGuildRoleDeleted(sender, args);
            };

            #endregion

            #region Invite

            Client.InviteCreated += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.InviteCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Invite.Code", args.Invite.Code.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordInviteEventsSubscribers())
                    await eventSubscriber.DiscordOnInviteCreated(sender, args);
            };

            Client.InviteDeleted += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.InviteDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Invite.Code", args.Invite.Code.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordInviteEventsSubscribers())
                    await eventSubscriber.DiscordOnInviteDeleted(sender, args);
            };

            #endregion

            #region Message

            Client.MessageCreated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                if (args.Guild != null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Author.Id", args.Author.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageCreated(sender, args);
            };

            Client.MessageAcknowledged += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageAcknowledged))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageAcknowledged(sender, args);
            };

            Client.MessageUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                if (args.Guild != null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Author.Id", args.Author.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageUpdated(sender, args);
            };

            Client.MessageDeleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                if (args.Guild != null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageDeleted(sender, args);
            };

            Client.MessagesBulkDeleted += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessagesBulkDeleted))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                if (args.Guild != null) workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageEventsSubscribers())
                    await eventSubscriber.DiscordOnMessagesBulkDeleted(sender, args);
            };

            #endregion

            #region Message Reaction

            Client.MessageReactionAdded += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageReactionAdded))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id);
                workScope.Span.SetTag("Channel.Id", args.Channel.Id);
                workScope.Span.SetTag("User.Id", args.User.Id);
                workScope.Span.SetTag("Message.Id", args.Message.Id);

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageReactionAddedEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageReactionAdded(sender, args);
            };

            Client.MessageReactionRemoved += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageReactionRemoved))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("User.Id", args.User.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageReactionAddedEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageReactionRemoved(sender, args);
            };

            Client.MessageReactionsCleared += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageReactionsCleared))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageReactionAddedEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageReactionsCleared(sender, args);
            };

            Client.MessageReactionRemovedEmoji += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.MessageReactionRemovedEmoji))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("Message.Id", args.Message.Id.ToString());
                workScope.Span.SetTag("Emoji.Id", args.Emoji.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMessageReactionAddedEventsSubscribers())
                    await eventSubscriber.DiscordOnMessageReactionRemovedEmoji(sender, args);
            };

            #endregion

            #region Presence/User Update

            Client.PresenceUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.PresenceUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("User.Id", args.User.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordPresenceUserEventsSubscribers())
                    await eventSubscriber.DiscordOnPresenceUpdated(sender, args);
            };

            Client.UserSettingsUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.UserSettingsUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("User.Id", args.User.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordPresenceUserEventsSubscribers())
                    await eventSubscriber.DiscordOnUserSettingsUpdated(sender, args);
            };

            Client.UserUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.UserUpdated)).IgnoreActiveSpan().StartActive(true);
                workScope.Span.SetTag("UserBefore.Id", args.UserBefore.Id.ToString());
                workScope.Span.SetTag("UserBefore.Id", args.UserBefore.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordPresenceUserEventsSubscribers())
                    await eventSubscriber.DiscordOnUserUpdated(sender, args);
            };

            #endregion

            #region Voice

            Client.VoiceStateUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.VoiceStateUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                if (args.Channel != null) workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("User.Id", args.User.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordVoiceEventsSubscribers())
                    await eventSubscriber.DiscordOnVoiceStateUpdated(sender, args);
            };

            Client.VoiceServerUpdated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.VoiceServerUpdated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordVoiceEventsSubscribers())
                    await eventSubscriber.DiscordOnVoiceServerUpdated(sender, args);
            };

            #endregion

            #region Misc

            Client.ComponentInteractionCreated += async (sender, args) =>
            {
                using var workScope = Tracer.BuildSpan(nameof(Client.ComponentInteractionCreated))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("Guild.Id", args.Guild.Id.ToString());
                workScope.Span.SetTag("Channel.Id", args.Channel.Id.ToString());
                workScope.Span.SetTag("User.Id", args.User.Id.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMiscEventsSubscribers())
                    await eventSubscriber.DiscordOnComponentInteractionCreated(sender, args);
            };

            Client.ClientErrored += async (sender, args) =>
            {
                using var workScope = Tracer
                    .BuildSpan(nameof(Client.ClientErrored))
                    .IgnoreActiveSpan()
                    .StartActive(true);
                workScope.Span.SetTag("EventName", args.EventName.ToString());

                using var scope = ServiceProvider.CreateScope();

                foreach (var eventSubscriber in scope.GetDiscordMiscEventsSubscribers())
                    await eventSubscriber.DiscordOnClientErrored(sender, args);
            };

            #endregion
        }
    }
}