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