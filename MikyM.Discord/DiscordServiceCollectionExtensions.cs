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
using DSharpPlus;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikyM.Discord.Attributes;
using MikyM.Discord.Interfaces;
using MikyM.Discord.Services;
using MikyM.Discord.Util;

namespace MikyM.Discord
{
    [UsedImplicitly]
    public static partial class DiscordServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers a <see cref="IDiscordService" /> with <see cref="DiscordConfiguration" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" />.</param>
        /// <param name="configure">The <see cref="DiscordConfiguration" />.</param>
        /// <param name="autoRegisterSubscribers">
        ///     If true, classes with subscriber attributes will get registered as event
        ///     subscribers automatically. This is the default.
        /// </param>
        /// <returns>The <see cref="IServiceCollection" />.</returns>
        [UsedImplicitly]
        public static IServiceCollection AddDiscord(
            this IServiceCollection services,
            Action<DiscordConfiguration> configure,
            bool autoRegisterSubscribers = true
        )
        {
            services.Configure(configure);

            services.TryAddSingleton<IDiscordService, DiscordService>();

            if (!autoRegisterSubscribers)
                return services;

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordChannelEventsSubscriberAttribute>())
                services.AddDiscordChannelEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordGuildBanEventsSubscriberAttribute>())
                services.AddDiscordGuildBanEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordGuildEventsSubscriberAttribute>())
                services.AddDiscordGuildEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordGuildMemberEventsSubscriberAttribute>())
                services.AddDiscordGuildMemberEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordGuildRoleEventsSubscriberAttribute>())
                services.AddDiscordGuildRoleEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordInviteEventsSubscriberAttribute>())
                services.AddDiscordInviteEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordMessageEventsSubscriberAttribute>())
                services.AddDiscordMessageEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordMessageReactionEventsSubscriberAttribute>())
                services.AddDiscordMessageReactionAddedEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordMiscEventsSubscriberAttribute>())
                services.AddDiscordMiscEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordPresenceUserEventsSubscriberAttribute>())
                services.AddDiscordPresenceUserEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordVoiceEventsSubscriberAttribute>())
                services.AddDiscordVoiceEventsSubscriber(type);

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordWebSocketEventSubscriberAttribute>())
                services.AddDiscordWebSocketEventSubscriber(type);

            return services;
        }

        /// <summary>
        ///     Registers a <see cref="DiscordHostedService" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" />.</param>
        /// <returns>The <see cref="IServiceCollection" />.</returns>
        [UsedImplicitly]
        public static IServiceCollection AddDiscordHostedService(
            this IServiceCollection services
        )
        {
            return services.AddHostedService<DiscordHostedService>();
        }
    }
}