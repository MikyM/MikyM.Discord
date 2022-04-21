// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
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

using DSharpPlus.Entities;
using MikyM.Discord.EmbedBuilders.Enrichers;

namespace MikyM.Discord.EmbedBuilders.Builders;

/// <inheritdoc cref="EnhancedDiscordEmbedBuilder"/>
/// <inheritdoc cref="IEnrichedDiscordEmbedBuilder"/>
/// <summary>
/// Constructs enriched embeds.
/// </summary>
public class EnrichedDiscordEmbedBuilder : EnhancedDiscordEmbedBuilder, IEnrichedDiscordEmbedBuilder
{
    /// <summary>
    /// Constructs an enriched embed builder.
    /// </summary>
    public EnrichedDiscordEmbedBuilder(){}

    /// <summary>
    /// Constructs an enriched embed builder from an existing <see cref="EnhancedDiscordEmbedBuilder"/>.
    /// </summary>
    /// <param name="builder">Builder to base this off of.</param>
    public EnrichedDiscordEmbedBuilder(EnhancedDiscordEmbedBuilder builder) : this(builder.GetCurrentInternal()) { }

    /// <summary>
    /// Constructs an enriched embed builder from an existing <see cref="EnrichedDiscordEmbedBuilder"/>.
    /// </summary>
    /// <param name="builder">Builder to base this off of.</param>
    public EnrichedDiscordEmbedBuilder(EnrichedDiscordEmbedBuilder builder) : this(builder.GetCurrentInternal()) { }

    /// <summary>
    /// Constructs an enriched embed builder.
    /// </summary>
    /// <param name="builder">Builder to base this off of.</param>
    public EnrichedDiscordEmbedBuilder(DiscordEmbedBuilder builder) : base(builder){}

    /// <inheritdoc />
    public virtual IEnrichedDiscordEmbedBuilder EnrichFrom<TEnricher>(TEnricher enricher)
        where TEnricher : IEmbedEnricher
    {
        enricher.Enrich(Current);
        return this;
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="builder"></param>
   /// <returns></returns>
   public static implicit operator DiscordEmbed(EnrichedDiscordEmbedBuilder builder)
        => builder.Build();
}
