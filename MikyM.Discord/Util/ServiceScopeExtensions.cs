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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MikyM.Discord.Events;

namespace MikyM.Discord.Util
{
    internal static class ServiceScopeExtensions
    {
        public static IList<IDiscordWebSocketEventsSubscriber> GetDiscordWebSocketEventSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordWebSocketEventsSubscriber))
                .Cast<IDiscordWebSocketEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordChannelEventsSubscriber> GetDiscordChannelEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordChannelEventsSubscriber))
                .Cast<IDiscordChannelEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordGuildEventsSubscriber> GetDiscordGuildEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordGuildEventsSubscriber))
                .Cast<IDiscordGuildEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordGuildBanEventsSubscriber> GetDiscordGuildBanEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordGuildBanEventsSubscriber))
                .Cast<IDiscordGuildBanEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordGuildMemberEventsSubscriber> GetDiscordGuildMemberEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordGuildMemberEventsSubscriber))
                .Cast<IDiscordGuildMemberEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordGuildRoleEventsSubscriber> GetDiscordGuildRoleEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordGuildRoleEventsSubscriber))
                .Cast<IDiscordGuildRoleEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordInviteEventsSubscriber> GetDiscordInviteEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordInviteEventsSubscriber))
                .Cast<IDiscordInviteEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordMessageEventsSubscriber> GetDiscordMessageEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordMessageEventsSubscriber))
                .Cast<IDiscordMessageEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordMessageReactionEventsSubscriber> GetDiscordMessageReactionAddedEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordMessageReactionEventsSubscriber))
                .Cast<IDiscordMessageReactionEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordPresenceUserEventsSubscriber> GetDiscordPresenceUserEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordPresenceUserEventsSubscriber))
                .Cast<IDiscordPresenceUserEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordVoiceEventsSubscriber> GetDiscordVoiceEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordVoiceEventsSubscriber))
                .Cast<IDiscordVoiceEventsSubscriber>()
                .ToList();
        }

        public static IList<IDiscordMiscEventsSubscriber> GetDiscordMiscEventsSubscribers(
            this IServiceScope scope
        )
        {
            return scope.ServiceProvider
                .GetServices(typeof(IDiscordMiscEventsSubscriber))
                .Cast<IDiscordMiscEventsSubscriber>()
                .ToList();
        }
    }
}