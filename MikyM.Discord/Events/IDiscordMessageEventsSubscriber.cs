// MIT License
//
// Copyright (c) 2021 Benjamin Höglinger-Stelzer
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