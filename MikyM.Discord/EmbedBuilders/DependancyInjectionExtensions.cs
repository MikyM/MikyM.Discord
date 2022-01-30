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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikyM.Common.Utilities.Extensions;
using MikyM.Discord.EmbedBuilders.Builders;

namespace MikyM.Discord.EmbedBuilders;

public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Registers <see cref="IEnhancedDiscordEmbedBuilder"/> with the <see cref="IServiceCollection"/>.
    /// <br></br><br></br>This method will also try to register other builders implementing <see cref="IEnhancedDiscordEmbedBuilder"/> with their concrete implementations by naming convention.
    /// </summary>
    public static void AddEnhancedDiscordEmbedBuilders(this IServiceCollection services)
    {
        var pairs = typeof(IEnhancedDiscordEmbedBuilder).GetInterfaceImplementationPairs();

        if (pairs.Count == 0) return;

        foreach (var (intr, impl) in pairs)
        {
            if (impl is null) continue;
            services.TryAddTransient(intr, impl);
        }
    }

    /// <summary>
    /// Registers <see cref="IEnrichedDiscordEmbedBuilder"/> with the <see cref="IServiceCollection"/>.
    /// <br></br><br></br>This method will also try to register other builders implementing <see cref="IEnrichedDiscordEmbedBuilder"/> with their concrete implementations by naming convention.
    /// <br></br><br></br><see cref="AddEnhancedDiscordEmbedBuilders"/> will also be automatically called.
    /// </summary>
    public static void AddEnrichedDiscordEmbedBuilders(this IServiceCollection services)
    {
        AddEnhancedDiscordEmbedBuilders(services);

        var pairs = typeof(IEnrichedDiscordEmbedBuilder).GetInterfaceImplementationPairs();

        if (pairs.Count == 0) return;


        foreach (var (intr, impl) in pairs)
        {
            if (impl is null) continue;
            services.TryAddTransient(intr, impl);
        }
    }
}