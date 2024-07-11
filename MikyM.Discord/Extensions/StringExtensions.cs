using System.Text.RegularExpressions;

namespace MikyM.Discord.Extensions;

/// <summary>
/// String extensions.
/// </summary>
[PublicAPI]
public static class StringExtensions
{
    /// <summary>
    /// Tries to parse a role mention.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="roleId">The role Id.</param>
    /// <returns>True if parsed successfully, otherwise false.</returns>
    public static bool TryParseRoleMention(this string value, out ulong roleId)
    {
        var res = Regex.Match(value, @"(?<=\\<@&)[0-9]{17,18}(?=\\>)");

        roleId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    /// <summary>
    /// Tries to parse a user mention.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="userId">The user Id.</param>
    /// <returns>True if parsed successfully, otherwise false.</returns>
    public static bool TryParseUserMention(this string value, out ulong userId)
    {
        var res = Regex.Match(value, @"(?<=\<@!|\<@)[0-9]{17,18}(?=\>)");

        userId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    /// <summary>
    /// Tries to parse a channel mention.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="channelId">The channel Id.</param>
    /// <returns>True if parsed successfully, otherwise false.</returns>
    public static bool TryParseChannelMention(this string value, out ulong channelId)
    {
        var res = Regex.Match(value, @"(?<=\<#)[0-9]{17,18}(?=\>)");

        channelId = res.Success ? ulong.Parse(res.Value) : 0;

        return res.Success;
    }

    /// <summary>
    /// Tries to parse a mention.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="id">The Id.</param>
    /// <returns>True if parsed successfully, otherwise false.</returns>
    public static bool TryParseDiscordMention(this string value, out ulong id)
    {
        return TryParseUserMention(value, out id) || TryParseChannelMention(value, out id) ||
               TryParseRoleMention(value, out id);
    }
}
