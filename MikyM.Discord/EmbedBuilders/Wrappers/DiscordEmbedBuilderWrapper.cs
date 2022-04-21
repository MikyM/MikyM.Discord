// This file is part of Lisbeth.Bot project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2021 DSharpPlus Contributors
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

namespace MikyM.Discord.EmbedBuilders.Wrappers;

/// <summary>
/// <see cref="DiscordEmbedBuilder"/> wrapper.
/// </summary>
public class DiscordEmbedBuilderWrapper : IDiscordEmbedBuilderWrapper
{
    /// <summary>
    /// Constructs a new <see cref="DiscordEmbedBuilderWrapper"/> with a new, clean <see cref="DiscordEmbedBuilder"/> instance inside.
    /// </summary>
    public DiscordEmbedBuilderWrapper()
        => Wrapped = new DiscordEmbedBuilder();

    /// <summary>
    /// Constructs a new <see cref="DiscordEmbedBuilderWrapper"/> based on a given <see cref="DiscordEmbedBuilder"/> instance.
    /// </summary>
    /// <param name="wrapped">Builder to wrap.</param>
    public DiscordEmbedBuilderWrapper(DiscordEmbedBuilder wrapped)
        => Wrapped = new DiscordEmbedBuilder(wrapped ?? throw new ArgumentNullException(nameof(wrapped)));

    /// <summary>
    /// Gets the current embed builder.
    /// </summary>
    protected DiscordEmbedBuilder Wrapped { get; }

    /// <summary>
    /// Builds the embed.
    /// </summary>
    protected internal DiscordEmbed Build() => Wrapped.Build();

    /// <summary>
    /// Extracts the builder.
    /// </summary>
    /// <returns>The instance of the currently wrapped builder.</returns>
    internal DiscordEmbedBuilder GetBaseInternal() => Wrapped;

    public IDiscordEmbedBuilderWrapper WithDescription(string description)
    {
        Wrapped.WithDescription(description);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithUrl(string url)
    {
        Wrapped.WithUrl(url);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithUrl(Uri url)
    {
        Wrapped.WithUrl(url);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithColor(DiscordColor color)
    {
        Wrapped.WithColor(color);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithTimestamp(DateTimeOffset? timestamp)
    {
        Wrapped.WithTimestamp(timestamp);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithTimestamp(DateTime? timestamp)
    {
        Wrapped.WithTimestamp(timestamp);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithTimestamp(ulong snowflake)
    {
        Wrapped.WithTimestamp(snowflake);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithImageUrl(string url)
    {
        Wrapped.WithImageUrl(url);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithImageUrl(Uri url)
    {
        Wrapped.WithImageUrl(url);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithThumbnail(string url, int height = 0, int width = 0)
    {
        Wrapped.WithThumbnail(url, height, width);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithThumbnail(Uri url, int height = 0, int width = 0)
    {
        Wrapped.WithThumbnail(url, height, width);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithAuthor(string? name = null, string? url = null, string? iconUrl = null)
    {
        Wrapped.WithAuthor(name, url, iconUrl);
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithFooter(string? text = null, string? iconUrl = null)
    {
        Wrapped.WithFooter(text, iconUrl);
        return this;
    }

    public IDiscordEmbedBuilderWrapper AddField(string name, string value, bool inline = false)
    {
        Wrapped.AddField(name, value, inline);
        return this;
    }

    public IDiscordEmbedBuilderWrapper RemoveFieldAt(int index)
    {
        Wrapped.RemoveFieldAt(index);
        return this;
    }

    public IDiscordEmbedBuilderWrapper RemoveFieldRange(int index, int count)
    {
        Wrapped.RemoveFieldRange(index, count);
        return this;
    }

    public IDiscordEmbedBuilderWrapper ClearFields()
    {
        Wrapped.ClearFields();
        return this;
    }

    public IDiscordEmbedBuilderWrapper WithTitle(string title)
    {
        Wrapped.WithTitle(title);
        return this;
    }
}