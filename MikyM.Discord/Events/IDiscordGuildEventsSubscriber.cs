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