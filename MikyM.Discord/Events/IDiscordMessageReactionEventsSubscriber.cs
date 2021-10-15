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
    public interface IDiscordMessageReactionEventsSubscriber
    {
        /// <summary>
        ///     Fired when a reaction gets added to a message.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessageReactions" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs args);

        /// <summary>
        ///     Fired when a reaction gets removed from a message.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessageReactions" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs args);

        /// <summary>
        ///     Fired when all reactions get removed from a message.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessageReactions" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageReactionsCleared(DiscordClient sender, MessageReactionsClearEventArgs args);

        /// <summary>
        ///     Fired when all reactions of a specific reaction are removed from a message.
        ///     For this Event you need the <see cref="DiscordIntents.GuildMessageReactions" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnMessageReactionRemovedEmoji(DiscordClient sender,
            MessageReactionRemoveEmojiEventArgs args);
    }
}