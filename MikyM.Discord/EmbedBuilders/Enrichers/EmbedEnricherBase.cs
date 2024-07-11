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

using MikyM.Discord.EmbedBuilders.Wrappers;

namespace MikyM.Discord.EmbedBuilders.Enrichers;

/// <summary>
/// Represents an embed enricher.
/// </summary>
/// <typeparam name="TPrimaryEnricher">Primary enricher.</typeparam>
/// <inheritdoc cref="IEmbedEnricher"/>
[PublicAPI]
public abstract class EmbedEnricherBase<TPrimaryEnricher> : IEmbedEnricher
{
    /// <summary>
    /// The primary enricher.
    /// </summary>
    protected TPrimaryEnricher PrimaryEnricher { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EmbedEnricherBase{TPrimaryEnricher}"/>.
    /// </summary>
    /// <param name="primaryEnricher">The primary enricher.</param>
    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher)
        => PrimaryEnricher = primaryEnricher;

    /// <summary>
    /// Enriches the embed.
    /// </summary>
    /// <param name="embedBuilder">The embed to enrich.</param>
    public abstract void Enrich(IDiscordEmbedBuilderWrapper embedBuilder);
}

/// <summary>
/// Represents an embed enricher.
/// </summary>
/// <typeparam name="TPrimaryEnricher">Primary enricher.</typeparam>
/// <typeparam name="TSecondaryEnricher">Secondary enricher.</typeparam>
/// <inheritdoc cref="IEmbedEnricher"/>
/// <inheritdoc cref="EmbedEnricherBase{T1}"/>
[PublicAPI]
public abstract class EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher> : EmbedEnricherBase<TPrimaryEnricher>
{
    /// <summary>
    /// The secondary enricher.
    /// </summary>
    protected TSecondaryEnricher SecondaryEnricher { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EmbedEnricherBase{TPrimaryEnricher, TSecondaryEnricher}"/>.
    /// </summary>
    /// <param name="primaryEnricher">The primary enricher.</param>
    /// <param name="secondaryEnricher">The seconary enricher.</param>
    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher) :
        base(primaryEnricher) =>
        SecondaryEnricher = secondaryEnricher;
}

/// <summary>
/// Represents an embed enricher.
/// </summary>
/// <typeparam name="TPrimaryEnricher">Primary enricher.</typeparam>
/// <typeparam name="TSecondaryEnricher">Secondary enricher.</typeparam>
/// <typeparam name="TTertiaryEnricher">Tertiary enricher.</typeparam>
/// <inheritdoc cref="IEmbedEnricher"/>
/// <inheritdoc cref="EmbedEnricherBase{T1,T2}"/>
[PublicAPI]
public abstract class
    EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher> : EmbedEnricherBase<TPrimaryEnricher,
        TSecondaryEnricher>
{
    /// <summary>
    /// The tertiary enricher.
    /// </summary>
    protected TTertiaryEnricher TertiaryEnricher { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EmbedEnricherBase{TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher}"/>.
    /// </summary>
    /// <param name="primaryEnricher">The primary enricher.</param>
    /// <param name="secondaryEnricher">The seconary enricher.</param>
    /// <param name="tertiaryEnricher">The tertiary enricher.</param>
    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher, TTertiaryEnricher tertiaryEnricher) :
        base(primaryEnricher, secondaryEnricher) =>
        TertiaryEnricher = tertiaryEnricher;
}

/// <summary>
/// Represents an embed enricher.
/// </summary>
/// <typeparam name="TPrimaryEnricher">Primary enricher.</typeparam>
/// <typeparam name="TSecondaryEnricher">Secondary enricher.</typeparam>
/// <typeparam name="TTertiaryEnricher">Tertiary enricher.</typeparam>
/// <typeparam name="TQuaternaryEnricher">Quaternary enricher.</typeparam>
/// <inheritdoc cref="IEmbedEnricher"/>
/// <inheritdoc cref="EmbedEnricherBase{T1,T2,T3}"/>
[PublicAPI]
public abstract class
    EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher, TQuaternaryEnricher> : EmbedEnricherBase<TPrimaryEnricher,
        TSecondaryEnricher, TTertiaryEnricher>
{
    /// <summary>
    /// The quaternary enricher.
    /// </summary>
    protected TQuaternaryEnricher QuaternaryEnricher { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EmbedEnricherBase{TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher, TQuaternaryEnricher}"/>.
    /// </summary>
    /// <param name="primaryEnricher">The primary enricher.</param>
    /// <param name="secondaryEnricher">The seconary enricher.</param>
    /// <param name="tertiaryEnricher">The tertiary enricher.</param>
    /// <param name="quaternaryEnricher">The quaternary enricher.</param>
    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher, TTertiaryEnricher tertiaryEnricher, TQuaternaryEnricher quaternaryEnricher) :
        base(primaryEnricher, secondaryEnricher, tertiaryEnricher) =>
        QuaternaryEnricher = quaternaryEnricher;
}
