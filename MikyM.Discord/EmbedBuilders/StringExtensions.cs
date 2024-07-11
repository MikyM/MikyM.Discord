using System.Text.RegularExpressions;

namespace MikyM.Discord.EmbedBuilders;

/// <summary>
/// String extensions.
/// </summary>
internal static class StringExtensions
{
    private static readonly Regex SplitRegex = new (@"(?<!^)(?=[A-Z])");

    internal static string SplitByCapitalAndConcat(this string value)
    {
        return string.Join(" ", SplitRegex.Split(value));
    }
}
