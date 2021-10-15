// MIT License
//
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