using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;

namespace MikyM.Discord.Extensions;

/// <summary>
/// Ulong extensions.
/// </summary>
[PublicAPI]
public static class UlongExtensions
{
    /// <summary>
    /// Checks if the bot is a member of the guild.
    /// </summary>
    /// <param name="guildId">The guild Id.</param>
    /// <param name="discord">The discord client.</param>
    /// <returns>True if the bot is a member of the guild, otherwise false.</returns>
    public static bool IsBotsMember(this ulong guildId, DiscordClient discord)
    {
        return discord.Guilds.Any(x =>
            x.Key == guildId);
    }

    /// <summary>
    /// Tries to get a guild from the cache.
    /// </summary>
    /// <param name="guildId">The guild Id.</param>
    /// <param name="discord">The client.</param>
    /// <param name="guild">The guild.</param>
    /// <returns>True if the guild was found in cache, otherwise false.</returns>
    public static bool TryGetGuildFromCache(this ulong guildId, DiscordClient discord, out DiscordGuild? guild)
    {
        return discord.Guilds.TryGetValue(
            guildId, out guild);
    }
}
