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

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace MikyM.Discord.Events
{
    public interface IDiscordGuildEventsSubscriber
    {
        /// <summary>
        ///     Fired when the user joins a new guild.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        /// <remarks>[alias="GuildJoined"][alias="JoinedGuild"]</remarks>
        public Task DiscordOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args);

        /// <summary>
        ///     Fired when a guild is becoming available.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildAvailable(DiscordClient sender, GuildCreateEventArgs args);

        /// <summary>
        ///     Fired when a guild is updated.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildUpdated(DiscordClient sender, GuildUpdateEventArgs args);

        /// <summary>
        ///     Fired when the user leaves or is removed from a guild.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildDeleted(DiscordClient sender, GuildDeleteEventArgs args);

        /// <summary>
        ///     Fired when a guild becomes unavailable.
        /// </summary>
        public Task DiscordOnGuildUnavailable(DiscordClient sender, GuildDeleteEventArgs args);

        /// <summary>
        ///     Fired when all guilds finish streaming from Discord.
        /// </summary>
        public Task DiscordOnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs args);

        /// <summary>
        ///     Fired when a guilds emojis get updated
        ///     For this Event you need the <see cref="DiscordIntents.GuildEmojis" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildEmojisUpdated(DiscordClient sender, GuildEmojisUpdateEventArgs args);

        public Task DiscordOnGuildStickersUpdated(DiscordClient sender, GuildStickersUpdateEventArgs args);

        /// <summary>
        ///     Fired when a guild integration is updated.
        /// </summary>
        public Task DiscordOnGuildIntegrationsUpdated(DiscordClient sender, GuildIntegrationsUpdateEventArgs args);
    }
}