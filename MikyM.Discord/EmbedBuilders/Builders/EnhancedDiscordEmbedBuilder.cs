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
using System.Reflection;
using DSharpPlus.Entities;
using MikyM.Discord.EmbedBuilders.Wrappers;
using MikyM.Discord.Extensions.BaseExtensions;

namespace MikyM.Discord.EmbedBuilders.Builders;

/// <inheritdoc />
public class EnhancedDiscordEmbedBuilder : IEnhancedDiscordEmbedBuilder
{
    /// <inheritdoc />
    public DiscordEmbed? Base { get; private set; }
    /// <inheritdoc />
    public DiscordEmbedBuilderWrapper Current { get; private set; }
    /// <inheritdoc />
    public string? Action { get; private set; }
    /// <inheritdoc />
    public string? ActionType { get; private set; }
    /// <inheritdoc />
    public long? CaseId { get; private set; }
    /// <inheritdoc />
    public DiscordUser? AuthorUser { get; private set; }
    /// <inheritdoc />
    public SnowflakeObject? FooterSnowflake { get; private set; }
    /// <inheritdoc />
    public string AuthorTemplate { get; private set; } = @"@action@@type@@info@"; // 0 - action , 1 - type, 2 - target/caller
    /// <inheritdoc />
    public string TitleTemplate { get; private set; } = @"@action@@type@@info@"; // 0 - action , 1 - type, 2 - target/caller
    /// <inheritdoc />
    public string FooterTemplate { get; private set; } = @"@caseId@@info@"; // 0 - caseId , 1 - snowflake info
    /// <inheritdoc />
    public bool IsTemplatingEnabled { get; private set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public EnhancedDiscordEmbedBuilder()
        => Current = new DiscordEmbedBuilderWrapper();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EnhancedDiscordEmbedBuilder(DiscordEmbedBuilder builder)
    {
        Base = new DiscordEmbedBuilder(builder ?? throw new ArgumentNullException(nameof(builder)));
        Current = new DiscordEmbedBuilderWrapper(builder);
    }

    /// <inheritdoc />
    public TBuilder AsEnriched<TBuilder>(params object[] args) where TBuilder : EnrichedDiscordEmbedBuilder
    {
        if (typeof(TBuilder) == typeof(EnrichedDiscordEmbedBuilder)) return (TBuilder)AsEnriched();

        if (!BuilderCache.CachedTypes.TryGetValue(typeof(TBuilder).FullName ?? throw new InvalidOperationException("Builder type is not valid."),
                out var type) || !typeof(TBuilder).IsAssignableFrom(type))
            throw new ArgumentException("Given builder type is not valid in this context.");

        var instance = Activator.CreateInstance(type,
            BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { args }, null);
        if (instance is null)
            throw new InvalidOperationException("Failed to create an instance of the specified builder.");

        var castInstance = instance as TBuilder;
        castInstance?.PrepareCustomEnriched(this);

        return castInstance ?? throw new InvalidOperationException("Failed to prepare the created builder instance."); ;
    }

    /// <inheritdoc />
    public IEnrichedDiscordEmbedBuilder AsEnriched()
        => new EnrichedDiscordEmbedBuilder(this);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static implicit operator DiscordEmbed(EnhancedDiscordEmbedBuilder builder)
        => builder.Build();

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder DisableTemplating(bool shouldDisableTemplating = true)
    {
        IsTemplatingEnabled = !shouldDisableTemplating;
        return this;
    }


    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder WithCase(long? caseId)
    {
        CaseId = caseId;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder WithEmbedColor(DiscordColor color)
    {
        Current.WithColor(color);
        return this;
    }

    internal DiscordEmbedBuilder GetCurrentInternal()
        => Current.GetBaseInternal();

    private void PrepareCustomEnriched(IEnhancedDiscordEmbedBuilder builderToBaseOffOf)
    {
        Current = builderToBaseOffOf.Current;
        Base = builderToBaseOffOf.Base;
    }

    /// <inheritdoc />
    public DiscordEmbedBuilder? ExtractBase() 
        => Base is null ? null : new DiscordEmbedBuilder(Current.GetBaseInternal());


    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder WithAuthorSnowflakeInfo(DiscordUser? user)
    {
        AuthorUser = user;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder SetAuthorTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        FooterTemplate = template;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder SetFooterTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        AuthorTemplate = template;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder SetTitleTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        TitleTemplate = template;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder WithFooterSnowflakeInfo(SnowflakeObject? snowflake)
    {
        FooterSnowflake = snowflake;
        return this;
    }

    /// <inheritdoc />
    public IEnhancedDiscordEmbedBuilder WithAction<TEnum>(TEnum action) where TEnum : Enum
    {
        Action = action.ToString();
        return this;
    }

    public IEnhancedDiscordEmbedBuilder WithActionType<TEnum>(TEnum actionType) where TEnum : Enum
    {
        ActionType = actionType.ToString();
        return this;
    }

    protected virtual void Evaluate()
    {
        if (!IsTemplatingEnabled) return;

        string author = AuthorTemplate
            .Replace("@action@", Action is null ? "" : Action.SplitByCapitalAndConcat())
            .Replace("@type@",
                ActionType is null
                    ? ""
                    : $" {ActionType.SplitByCapitalAndConcat()}");

        author = author.Replace("@info@",
            AuthorUser is null ? "" : $" | {AuthorUser.GetFullUsername()}");

        Current.WithAuthor(author, null, AuthorUser?.AvatarUrl);

        if (FooterSnowflake is null && !CaseId.HasValue) return;
        string footer = FooterTemplate.Replace("@caseId@", CaseId is null ? "" : $"Case Id: {CaseId}")
            .Replace("@info@",
                FooterSnowflake is null
                    ? ""
                    : $"{(CaseId.HasValue ? " | " : "")}{FooterSnowflake.GetType().Name.SplitByCapitalAndConcat()} Id: {FooterSnowflake.Id}");

        Current.WithFooter(footer);
    }

    public DiscordEmbed Build()
    {
        Evaluate();
        return Current.Build();
    }
}
