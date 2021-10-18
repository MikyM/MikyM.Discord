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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MikyM.Discord.Events;

namespace MikyM.Discord
{
    public static partial class DiscordServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddDiscordWebSocketEventSubscriber<T>(this IServiceCollection services)
            where T : IDiscordWebSocketEventsSubscriber
        {
            return services.AddDiscordWebSocketEventSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordWebSocketEventSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordWebSocketEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordChannelEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordChannelEventsSubscriber
        {
            return services.AddDiscordChannelEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordChannelEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordChannelEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordGuildEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordGuildEventsSubscriber
        {
            return services.AddDiscordGuildEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordGuildEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordGuildEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordGuildBanEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordGuildBanEventsSubscriber
        {
            return services.AddDiscordGuildBanEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordGuildBanEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordGuildBanEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordGuildMemberEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordGuildMemberEventsSubscriber
        {
            return services.AddDiscordGuildMemberEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordGuildMemberEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordGuildMemberEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordGuildRoleEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordGuildRoleEventsSubscriber
        {
            return services.AddDiscordGuildRoleEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordGuildRoleEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordGuildRoleEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordInviteEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordInviteEventsSubscriber
        {
            return services.AddDiscordInviteEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordInviteEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordInviteEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordMessageEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordMessageEventsSubscriber
        {
            return services.AddDiscordMessageEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordMessageEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordMessageEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordMessageReactionAddedEventsSubscriber<T>(
            this IServiceCollection services)
            where T : IDiscordMessageReactionEventsSubscriber
        {
            return services.AddDiscordMessageReactionAddedEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordMessageReactionAddedEventsSubscriber(
            this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordMessageReactionEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordPresenceUserEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordPresenceUserEventsSubscriber
        {
            return services.AddDiscordPresenceUserEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordPresenceUserEventsSubscriber(this IServiceCollection services,
            Type t)
        {
            return services.AddScoped(typeof(IDiscordPresenceUserEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordVoiceEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordVoiceEventsSubscriber
        {
            return services.AddDiscordVoiceEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordVoiceEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordVoiceEventsSubscriber), t);
        }

        [UsedImplicitly]
        public static IServiceCollection AddDiscordMiscEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordMiscEventsSubscriber
        {
            return services.AddDiscordMiscEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordMiscEventsSubscriber(this IServiceCollection services, Type t)
        {
            return services.AddScoped(typeof(IDiscordMiscEventsSubscriber), t);
        }
    }
}