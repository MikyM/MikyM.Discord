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
using Autofac;
using MikyM.Discord.Events;

namespace MikyM.Discord.Util;

internal static class LifetimeScopeExtensions
{
    public static IList<IDiscordWebSocketEventsSubscriber> GetDiscordWebSocketEventSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordWebSocketEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordChannelEventsSubscriber> GetDiscordChannelEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable< IDiscordChannelEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordGuildEventsSubscriber> GetDiscordGuildEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordGuildEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordGuildBanEventsSubscriber> GetDiscordGuildBanEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordGuildBanEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordGuildMemberEventsSubscriber> GetDiscordGuildMemberEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordGuildMemberEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordGuildRoleEventsSubscriber> GetDiscordGuildRoleEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordGuildRoleEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordInviteEventsSubscriber> GetDiscordInviteEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordInviteEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordMessageEventsSubscriber> GetDiscordMessageEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordMessageEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordMessageReactionEventsSubscriber> GetDiscordMessageReactionAddedEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordMessageReactionEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordPresenceUserEventsSubscriber> GetDiscordPresenceUserEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordPresenceUserEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordVoiceEventsSubscriber> GetDiscordVoiceEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordVoiceEventsSubscriber>>()
            .ToList();
    }

    public static IList<IDiscordMiscEventsSubscriber> GetDiscordMiscEventsSubscribers(
        this ILifetimeScope scope
    )
    {
        return scope.Resolve<IEnumerable<IDiscordMiscEventsSubscriber>>()
            .ToList();
    }
}