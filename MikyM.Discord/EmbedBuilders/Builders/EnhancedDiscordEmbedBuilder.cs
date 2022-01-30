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
using MikyM.Discord.EmbedBuilders.Wrappers;
using MikyM.Discord.Extensions.BaseExtensions;
using System;

namespace MikyM.Discord.EmbedBuilders.Builders;

public class EnhancedDiscordEmbedBuilder : IEnhancedDiscordEmbedBuilder
{
    public DiscordEmbed? Base { get; private set; }
    public DiscordEmbedBuilderWrapper Current { get; private set; }
    public string? Action { get; private set; }
    public string? ActionType { get; private set; }
    public long? CaseId { get; private set; }
    public DiscordUser? AuthorUser { get; private set; }
    public SnowflakeObject? FooterSnowflake { get; private set; }
    public string AuthorTemplate { get; private set; } = @"@action@@type@@info@"; // 0 - action , 1 - type, 2 - target/caller
    public string TitleTemplate { get; private set; } = @"@action@@type@@info@"; // 0 - action , 1 - type, 2 - target/caller
    public string FooterTemplate { get; private set; } = @"@caseId@@info@"; // 0 - caseId , 1 - snowflake info
    public bool IsTemplatingEnabled { get; private set; } = true;

    public EnhancedDiscordEmbedBuilder()
        => this.Current = new DiscordEmbedBuilderWrapper();

    public EnhancedDiscordEmbedBuilder(DiscordEmbedBuilder builder)
    {
        this.Base = new DiscordEmbedBuilder(builder ?? throw new ArgumentNullException(nameof(builder)));
        this.Current = new DiscordEmbedBuilderWrapper(builder);
    }

    public TBuilder AsEnriched<TBuilder>(params object[] args) where TBuilder : EnrichedDiscordEmbedBuilder
    {
        if (typeof(TBuilder) == typeof(EnrichedDiscordEmbedBuilder)) return (TBuilder)this.AsEnriched();

        if (!BuilderCache.CachedTypes.TryGetValue(typeof(TBuilder).FullName ?? throw new InvalidOperationException("Builder type is not valid."),
                out var type) || !typeof(TBuilder).IsAssignableFrom(type))
            throw new ArgumentException("Given builder type is not valid in this context.");

        var instance = Activator.CreateInstance(type,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new object[] { args }, null);
        if (instance is null)
            throw new InvalidOperationException("Failed to create an instance of the specified builder.");

        var castInstance = instance as TBuilder;
        castInstance?.PrepareCustomEnriched(this);

        return castInstance ?? throw new InvalidOperationException("Failed to prepare the created builder instance."); ;
    }

    public IEnrichedDiscordEmbedBuilder AsEnriched()
        => new EnrichedDiscordEmbedBuilder(this);

    public static implicit operator DiscordEmbed(EnhancedDiscordEmbedBuilder builder)
        => builder.Build();

    public IEnhancedDiscordEmbedBuilder DisableTemplating(bool shouldDisableTemplating = true)
    {
        this.IsTemplatingEnabled = !shouldDisableTemplating;
        return this;
    }


    public IEnhancedDiscordEmbedBuilder WithCase(long? caseId)
    {
        this.CaseId = caseId;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder WithEmbedColor(DiscordColor color)
    {
        this.Current.WithColor(color);
        return this;
    }

    internal DiscordEmbedBuilder GetCurrentInternal()
        => this.Current.GetBaseInternal();

    private void PrepareCustomEnriched(IEnhancedDiscordEmbedBuilder builderToBaseOffOf)
    {
        this.Current = builderToBaseOffOf.Current;
        this.Base = builderToBaseOffOf.Base;
    }

    public DiscordEmbedBuilder? ExtractBase() 
        => this.Base is null ? null : new DiscordEmbedBuilder(this.Current.GetBaseInternal());
    

    public IEnhancedDiscordEmbedBuilder WithAuthorSnowflakeInfo(DiscordUser? user)
    {
        this.AuthorUser = user;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder SetAuthorTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        this.FooterTemplate = template;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder SetFooterTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        this.AuthorTemplate = template;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder SetTitleTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("Invalid template", nameof(template));
        this.TitleTemplate = template;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder WithFooterSnowflakeInfo(SnowflakeObject? snowflake)
    {
        this.FooterSnowflake = snowflake;
        return this;
    }

    public IEnhancedDiscordEmbedBuilder WithAction<TEnum>(TEnum action) where TEnum : Enum
    {
        this.Action = action.ToString();
        return this;
    }

    public IEnhancedDiscordEmbedBuilder WithActionType<TEnum>(TEnum actionType) where TEnum : Enum
    {
        this.ActionType = actionType.ToString();
        return this;
    }

    protected virtual void Evaluate()
    {
        if (!this.IsTemplatingEnabled) return;

        string author = this.AuthorTemplate
            .Replace("@action@", this.Action is null ? "" : this.Action.SplitByCapitalAndConcat())
            .Replace("@type@",
                this.ActionType is null
                    ? ""
                    : $" {this.ActionType.SplitByCapitalAndConcat()}");

        author = author.Replace("@info@",
            this.AuthorUser is null ? "" : $" | {this.AuthorUser.GetFullUsername()}");

        this.Current.WithAuthor(author, null, this.AuthorUser?.AvatarUrl);

        if (this.FooterSnowflake is null && !this.CaseId.HasValue) return;
        string footer = this.FooterTemplate.Replace("@caseId@", this.CaseId is null ? "" : $"Case Id: {this.CaseId}")
            .Replace("@info@",
                this.FooterSnowflake is null
                    ? ""
                    : $"{(this.CaseId.HasValue ? " | " : "")}{this.FooterSnowflake.GetType().Name.SplitByCapitalAndConcat()} Id: {this.FooterSnowflake.Id}");

        this.Current.WithFooter(footer);
    }

    public DiscordEmbed Build()
    {
        this.Evaluate();
        return this.Current.Build();
    }
}
