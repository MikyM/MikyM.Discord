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

using System.Linq;
using DSharpPlus.Entities;
using MikyM.Discord.EmbedBuilders.Builders;

namespace MikyM.Discord.EmbedBuilders;

public static class DiscordEmbedBuilderExtensions
{
    /// <summary> Determines whether a given <see cref="DiscordEmbedBuilder"/> is valid regarding all fields combined length being less than 6000 characters long.</summary>
    /// <returns> Validation result </returns>
    public static bool IsValid(this DiscordEmbedBuilder builder)
        => !(builder.Author?.Name.Length + builder.Footer?.Text.Length + builder.Description?.Length +
            builder.Title?.Length + builder.Fields?.Sum(x => x.Value.Length + x.Name.Length) > 6000);

    /// <summary> Creates an instance of <see cref="EnhancedDiscordEmbedBuilder"/> based on given <see cref="DiscordEmbedBuilder"/>.</summary>
    /// <returns> New instance of <see cref="EnhancedDiscordEmbedBuilder"/>. </returns>
    public static IEnhancedDiscordEmbedBuilder AsEnhanced(this DiscordEmbedBuilder builder) 
        => new EnhancedDiscordEmbedBuilder(builder);

    /// <summary> Creates an instance of <see cref="EnrichedDiscordEmbedBuilder"/> based on given <see cref="DiscordEmbedBuilder"/>.</summary>
    /// <returns> New instance of <see cref="EnrichedDiscordEmbedBuilder"/>. </returns>
    public static IEnhancedDiscordEmbedBuilder AsEnriched(this DiscordEmbedBuilder builder) 
        => new EnrichedDiscordEmbedBuilder(builder);
}