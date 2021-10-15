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
    public interface IDiscordGuildMemberEventsSubscriber
    {
        /// <summary>
        ///     Fired when a new user joins a guild.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMembers" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs args);

        /// <summary>
        ///     Fired when a user is removed from a guild (leave/kick/ban).
        ///     For this Event you need the <see cref="DiscordIntents.GuildMembers" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs args);

        /// <summary>
        ///     Fired when a guild member is updated.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMembers" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnGuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs args);

        /// <summary>
        ///     Fired in response to Gateway Request Guild Members.
        /// </summary>
        public Task DiscordOnGuildMembersChunked(DiscordClient sender, GuildMembersChunkEventArgs args);
    }
}