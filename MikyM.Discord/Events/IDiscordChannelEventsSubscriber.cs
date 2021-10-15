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
    public interface IDiscordChannelEventsSubscriber
    {
        /// <summary>
        ///     Fired when a new channel is created.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnChannelCreated(DiscordClient sender, ChannelCreateEventArgs args);

        /// <summary>
        ///     Fired when a channel is updated.
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnChannelUpdated(DiscordClient sender, ChannelUpdateEventArgs args);

        /// <summary>
        ///     Fired when a channel is deleted
        ///     For this Event you need the <see cref="DiscordIntents.Guilds" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs args);

        /// <summary>
        ///     Fired when a dm channel is deleted
        ///     For this Event you need the <see cref="DiscordIntents.DirectMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnDmChannelDeleted(DiscordClient sender, DmChannelDeleteEventArgs args);

        /// <summary>
        ///     Fired when a dm channel is deleted
        ///     For this Event you need the <see cref="DiscordIntents.DirectMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnChannelPinsUpdated(DiscordClient sender, ChannelPinsUpdateEventArgs args);
    }
}