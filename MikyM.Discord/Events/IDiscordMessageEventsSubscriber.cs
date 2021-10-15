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
    public interface IDiscordMessageEventsSubscriber
    {
        /// <summary>
        ///     Fired when a message is created.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageCreated(DiscordClient sender, MessageCreateEventArgs args);

        /// <summary>
        ///     Fired when message is acknowledged by the user.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageAcknowledged(DiscordClient sender, MessageAcknowledgeEventArgs args);

        /// <summary>
        ///     Fired when a message is updated.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageUpdated(DiscordClient sender, MessageUpdateEventArgs args);

        /// <summary>
        ///     Fired when a message is deleted.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageDeleted(DiscordClient sender, MessageDeleteEventArgs args);

        /// <summary>
        ///     Fired when multiple messages are deleted at once.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessages" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessagesBulkDeleted(DiscordClient sender, MessageBulkDeleteEventArgs args);
    }
}