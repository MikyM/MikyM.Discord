using DSharpPlus.Entities;

namespace MikyM.Discord.EmbedBuilders.Builders;

/// <summary>
/// Represents a base embed builder.
/// </summary>
[PublicAPI]
public interface IBaseEmbedBuilder
{
    /// <summary>
    /// Constructs a new embed from data supplied to this builder.
    /// </summary>
    /// <returns>New discord embed.</returns>
    DiscordEmbed Build();
}
