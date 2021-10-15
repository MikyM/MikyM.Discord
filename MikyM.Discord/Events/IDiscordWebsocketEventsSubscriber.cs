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
    public interface IDiscordWebSocketEventsSubscriber
    {
        /// <summary>
        ///     Fired whenever a WebSocket error occurs within the client.
        /// </summary>
        public Task DiscordOnSocketErrored(DiscordClient sender, SocketErrorEventArgs args);

        /// <summary>
        ///     Fired whenever WebSocket connection is established.
        /// </summary>
        public Task DiscordOnSocketOpened(DiscordClient sender, SocketEventArgs args);

        /// <summary>
        ///     Fired whenever WebSocket connection is terminated.
        /// </summary>
        public Task DiscordOnSocketClosed(DiscordClient sender, SocketCloseEventArgs args);

        /// <summary>
        ///     Fired when the client enters ready state.
        /// </summary>
        public Task DiscordOnReady(DiscordClient sender, ReadyEventArgs args);

        /// <summary>
        ///     Fired whenever a session is resumed.
        /// </summary>
        public Task DiscordOnResumed(DiscordClient sender, ReadyEventArgs args);

        /// <summary>
        ///     Fired on received heartbeat ACK.
        /// </summary>
        public Task DiscordOnHeartbeated(DiscordClient sender, HeartbeatEventArgs args);

        /// <summary>
        ///     Fired on heartbeat attempt cancellation due to too many failed heartbeats.
        /// </summary>
        public Task DiscordOnZombied(DiscordClient sender, ZombiedEventArgs args);
    }
}