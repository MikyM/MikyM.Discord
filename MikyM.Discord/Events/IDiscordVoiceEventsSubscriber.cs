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
    public interface IDiscordVoiceEventsSubscriber
    {
        /// <summary>
        ///     Fired when someone joins/leaves/moves voice channels.
        ///     For this Event you need the <see cref="DiscordIntents.GuildVoiceStates" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args);

        /// <summary>
        ///     Fired when a guild's voice server is updated.
        ///     For this Event you need the <see cref="DiscordIntents.GuildVoiceStates" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnVoiceServerUpdated(DiscordClient sender, VoiceServerUpdateEventArgs args);
    }
}