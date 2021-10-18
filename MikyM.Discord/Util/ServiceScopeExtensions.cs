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