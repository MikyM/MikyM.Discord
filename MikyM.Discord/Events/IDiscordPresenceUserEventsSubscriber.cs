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
    public interface IDiscordPresenceUserEventsSubscriber
    {
        /// <summary>
        ///     Fired when a presence has been updated.
        ///     For this Event you need the <see cref="DiscordIntents.GuildPresences" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnPresenceUpdated(DiscordClient sender, PresenceUpdateEventArgs args);

        /// <summary>
        ///     Fired when the current user updates their settings.
        ///     For this Event you need the <see cref="DiscordIntents.GuildPresences" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </summary>
        public Task DiscordOnUserSettingsUpdated(DiscordClient sender, UserSettingsUpdateEventArgs args);

        /// <summary>
        ///     Fired when properties about the current user change.
        /// </summary>
        /// <remarks>
        ///     NB: This event only applies for changes to the <b>current user</b>, the client that is connected to Discord.
        ///     For this Event you need the <see cref="DiscordIntents.GuildPresences" /> intent specified in
        ///     <seealso cref="DiscordConfiguration.Intents" />
        /// </remarks>
        public Task DiscordOnUserUpdated(DiscordClient sender, UserUpdateEventArgs args);
    }
}