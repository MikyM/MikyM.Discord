// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General License for more details.
// 
// You should have received a copy of the GNU Affero General License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using DSharpPlus.Entities;

namespace MikyM.Discord.EmbedBuilders.Wrappers;

/// <summary>
/// <see cref="DiscordEmbedBuilder"/> wrapper.
/// </summary>
public interface IDiscordEmbedBuilderWrapper
{
    /// <summary>
    /// Sets the embed's description.
    /// </summary>
    /// <param name="description">Description to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithDescription(string description);

    /// <summary>
    /// Sets the embed's title url.
    /// </summary>
    /// <param name="url">Title url to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithUrl(string url);

    /// <summary>
    /// Sets the embed's title url.
    /// </summary>
    /// <param name="url">Title url to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithUrl(Uri url);


    /// <summary>
    /// Sets the embed's color.
    /// </summary>
    /// <param name="color">Embed color to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithColor(DiscordColor color);

    /// <summary>
    /// Sets the embed's timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithTimestamp(DateTimeOffset? timestamp);


    /// <summary>
    /// Sets the embed's timestamp.
    /// </summary>
    /// <param name="timestamp">Timestamp to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithTimestamp(DateTime? timestamp);

    /// <summary>
    /// Sets the embed's timestamp based on a snowflake.
    /// </summary>
    /// <param name="snowflake">Snowflake to calculate timestamp from.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithTimestamp(ulong snowflake);

    /// <summary>
    /// Sets the embed's image url.
    /// </summary>
    /// <param name="url">Image url to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithImageUrl(string url);

    /// <summary>
    /// Sets the embed's image url.
    /// </summary>
    /// <param name="url">Image url to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithImageUrl(Uri url);

    /// <summary>
    /// Sets the embed's thumbnail.
    /// </summary>
    /// <param name="url">Thumbnail url to set.</param>
    /// <param name="height">The height of the thumbnail to set.</param>
    /// <param name="width">The width of the thumbnail to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithThumbnail(string url, int height = 0, int width = 0);

    /// <summary>
    /// Sets the embed's thumbnail.
    /// </summary>
    /// <param name="url">Thumbnail url to set.</param>
    /// <param name="height">The height of the thumbnail to set.</param>
    /// <param name="width">The width of the thumbnail to set.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithThumbnail(Uri url, int height = 0, int width = 0);

    /// <summary>
    /// Sets the embed's author.
    /// </summary>
    /// <param name="name">Author's name.</param>
    /// <param name="url">Author's url.</param>
    /// <param name="iconUrl">Author icon's url.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithAuthor(string? name = null, string? url = null, string? iconUrl = null);

    /// <summary>
    /// Sets the embed's footer.
    /// </summary>
    /// <param name="text">Footer's text.</param>
    /// <param name="iconUrl">Footer icon's url.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithFooter(string? text = null, string? iconUrl = null);

    /// <summary>
    /// Adds a field to this embed.
    /// </summary>
    /// <param name="name">Name of the field to add.</param>
    /// <param name="value">Value of the field to add.</param>
    /// <param name="inline">Whether the field is to be inline or not.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper AddField(string name, string value, bool inline = false);

    /// <summary>
    /// Removes a field of the specified index from this embed.
    /// </summary>
    /// <param name="index">Index of the field to remove.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper RemoveFieldAt(int index);

    /// <summary>
    /// Removes fields of the specified range from this embed.
    /// </summary>
    /// <param name="index">Index of the first field to remove.</param>
    /// <param name="count">Number of fields to remove.</param>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper RemoveFieldRange(int index, int count);

    /// <summary>
    /// Removes all fields from this embed.
    /// </summary>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper ClearFields();

    /// <summary>
    /// Sets this embed's title.
    /// </summary>
    /// <returns>This embed builder wrapper.</returns>
    IDiscordEmbedBuilderWrapper WithTitle(string title);
}