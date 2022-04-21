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

using System;
using DSharpPlus.Entities;
using MikyM.Discord.EmbedBuilders.Wrappers;

namespace MikyM.Discord.EmbedBuilders.Builders;

public interface IEnhancedDiscordEmbedBuilder : IBaseEmbedBuilder
{
    /// <summary> Gets base result built from the passed <see cref="DiscordEmbedBuilder"/>, if any.</summary>
    DiscordEmbed? Base { get; }
    /// <summary> Gets wrapper for current <see cref="DiscordEmbedBuilder"/> in use and gives access to it's methods.</summary>
    DiscordEmbedBuilderWrapper Current { get; }
    /// <summary> Gets template for <see cref="DiscordEmbedBuilder.EmbedAuthor"/> field.</summary>
    string AuthorTemplate { get; }
    /// <summary> Gets template for <see cref="DiscordEmbedBuilder.Title"/> field.</summary>
    string TitleTemplate { get; }
    /// <summary> Gets template for <see cref="DiscordEmbedBuilder.EmbedFooter"/> field.</summary>
    string FooterTemplate { get; }
    /// <summary> Gets case Id, if any.</summary>
    long? CaseId { get; }
    /// <summary> Gets <see cref="DiscordMember"/> used for <see cref="DiscordEmbedBuilder.EmbedAuthor"/> templating, if any.</summary>
    DiscordUser? AuthorUser { get; }
    /// <summary> Gets <see cref="SnowflakeObject"/> used for <see cref="DiscordEmbedBuilder.EmbedFooter"/> templating, if any.</summary>
    SnowflakeObject? FooterSnowflake { get; }
    /// <summary> Gets action type name used for <see cref="DiscordEmbedBuilder.EmbedAuthor"/> templating, if any.</summary>
    string? ActionType { get; }
    /// <summary> Gets action name used for <see cref="DiscordEmbedBuilder.EmbedAuthor"/> templating, if any.</summary>
    string? Action { get; }
    /// <summary> Gets whether templating of author and footer fields is enabled (default - enabled).</summary>
    public bool IsTemplatingEnabled { get; }

    /// <summary> Sets whether the author and footer field templating should be enabled. </summary>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder DisableTemplating(bool shouldDisableTemplating = true);
    /// <summary> Sets the embed color for this builder. </summary>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithEmbedColor(DiscordColor color);
    /// <summary> Sets the enums string representation to be used in the author template as the name of the action performed. </summary>
    /// <param name="action">The action being performed.</param>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithAction<TEnum>(TEnum action) where TEnum : Enum;
    /// <summary> Sets the enums string representation to be used in the author template as the name of the type of the action performed. </summary>
    /// <param name="actionType">The type of the action being performed.</param>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithActionType<TEnum>(TEnum actionType) where TEnum : Enum;
    /// <summary> Sets the caseId to be used in the footer template. </summary>
    /// <param name="caseId">The case Id of the action being performed.</param>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithCase(long? caseId);
    /// <summary> Sets the <see cref="SnowflakeObject"/> to be used in the footer template. </summary>
    /// <param name="snowflake">Target of the action.</param>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithFooterSnowflakeInfo(SnowflakeObject? snowflake);
    /// <summary> Sets the <see cref="DiscordUser"/> to be used in the author template. </summary>
    /// <param name="member">Target, cause or caller of the action.</param>
    /// <returns>The current builder instance.</returns>
    IEnhancedDiscordEmbedBuilder WithAuthorSnowflakeInfo(DiscordUser? user);
    /// <summary> Sets the author template. </summary>
    /// <returns>The current builder instance.</returns>
    /// <param name="template">Author template.</param>
    IEnhancedDiscordEmbedBuilder SetAuthorTemplate(string template);
    /// <summary> Sets the footer template. </summary>
    /// <returns>The current builder instance.</returns>
    /// <param name="template">Footer template.</param>
    IEnhancedDiscordEmbedBuilder SetFooterTemplate(string template);
    /// <summary> Sets the title template. </summary>
    /// <returns>The current builder instance.</returns>
    /// <param name="template">Title template.</param>
    IEnhancedDiscordEmbedBuilder SetTitleTemplate(string template);
    /// <summary> Extracts the builder. </summary>
    /// <returns>A new instance of <see cref="DiscordEmbedBuilder"/> based off of the base builder.</returns>
    DiscordEmbedBuilder? ExtractBase();
    /// <summary> Creates a new instance of a specified enriched builder based on this builder.</summary>
    /// <returns>An instance of a specified enriched builder.</returns>
    /// <param name="args">Constructor arguments to be passed.</param>
    TBuilder AsEnriched<TBuilder>(params object[] args) where TBuilder : EnrichedDiscordEmbedBuilder;
    /// <summary> Creates a new instance of an enriched builder based on this builder. </summary>
    /// <returns>An instance of an enriched builder.</returns>
    IEnrichedDiscordEmbedBuilder AsEnriched();
}