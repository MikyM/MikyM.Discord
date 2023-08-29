// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
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

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using MikyM.Discord.Events;

namespace MikyM.Discord.Util;

public class ReadyToOperateHandler : IDiscordGuildEventsSubscriber
{
    public Task DiscordOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildAvailable(DiscordClient sender, GuildCreateEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildUpdated(DiscordClient sender, GuildUpdateEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildDeleted(DiscordClient sender, GuildDeleteEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildUnavailable(DiscordClient sender, GuildDeleteEventArgs args)
    {
        return Task.CompletedTask;
    }

    public async Task DiscordOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs args)
    {
        if (!WaitForDownloadCompletionHandler.Instance.ReadyToOperateEvent.IsSet)
            await WaitForDownloadCompletionHandler.Instance.ReadyToOperateEvent.SetAsync();
    }

    public Task DiscordOnGuildEmojisUpdated(DiscordClient sender, GuildEmojisUpdateEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildStickersUpdated(DiscordClient sender, GuildStickersUpdateEventArgs args)
    {
        return Task.CompletedTask;
    }

    public Task DiscordOnGuildIntegrationsUpdated(DiscordClient sender, GuildIntegrationsUpdateEventArgs args)
    {
        return Task.CompletedTask;
    }
}
