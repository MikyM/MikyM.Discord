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