using MikyM.Discord.EmbedBuilders.Wrappers;

namespace MikyM.Discord.EmbedBuilders.Enrichers;

/// <summary>
/// Represents an embed enricher.
/// </summary>
[PublicAPI]
public interface IEmbedEnricher
{
    /// <summary>
    /// Enriches the embed.
    /// </summary>
    /// <param name="embedBuilder">The builder.</param>
    void Enrich(IDiscordEmbedBuilderWrapper embedBuilder);
}
