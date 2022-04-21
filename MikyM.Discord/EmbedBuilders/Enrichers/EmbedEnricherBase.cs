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

public abstract class EmbedEnricherBase<TPrimaryEnricher> : IEmbedEnricher
{
    protected TPrimaryEnricher PrimaryEnricher { get; }

    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher)
        => PrimaryEnricher = primaryEnricher;

    public abstract void Enrich(IDiscordEmbedBuilderWrapper embedBuilder);
}

public abstract class EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher> : EmbedEnricherBase<TPrimaryEnricher>
{
    protected TSecondaryEnricher SecondaryEnricher { get; }

    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher) :
        base(primaryEnricher) =>
        SecondaryEnricher = secondaryEnricher;
}

public abstract class
    EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher> : EmbedEnricherBase<TPrimaryEnricher,
        TSecondaryEnricher>
{
    protected TTertiaryEnricher TertiaryEnricher { get; }

    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher, TTertiaryEnricher tertiaryEnricher) :
        base(primaryEnricher, secondaryEnricher) =>
        TertiaryEnricher = tertiaryEnricher;
}

public abstract class
    EmbedEnricherBase<TPrimaryEnricher, TSecondaryEnricher, TTertiaryEnricher, TQuaternaryEnricher> : EmbedEnricherBase<TPrimaryEnricher,
        TSecondaryEnricher, TTertiaryEnricher>
{
    protected TQuaternaryEnricher QuaternaryEnricher { get; }

    protected EmbedEnricherBase(TPrimaryEnricher primaryEnricher, TSecondaryEnricher secondaryEnricher, TTertiaryEnricher tertiaryEnricher, TQuaternaryEnricher quaternaryEnricher) :
        base(primaryEnricher, secondaryEnricher, tertiaryEnricher) =>
        QuaternaryEnricher = quaternaryEnricher;
}
