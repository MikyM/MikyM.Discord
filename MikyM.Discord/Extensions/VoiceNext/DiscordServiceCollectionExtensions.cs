// MIT License
//
// Copyright (c) 2021 Benjamin Höglinger-Stelzer
// Copyright (c) 2021 Krzysztof Kupisz - MikyM
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using DSharpPlus.VoiceNext;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MikyM.Discord.Interfaces;

namespace MikyM.Discord.Extensions.VoiceNext;

[UsedImplicitly]
public static class DiscordServiceCollectionExtensions
{
    /// <summary>
    ///     Adds VoiceNext extension to <see cref="IDiscordService" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configure">The <see cref="VoiceNextConfiguration" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [UsedImplicitly]
    public static IServiceCollection AddDiscordVoiceNext(
        this IServiceCollection services,
        Action<VoiceNextConfiguration?>? configure = null
    )
    {
        services.AddSingleton(typeof(IDiscordExtensionConfiguration), provider =>
        {
            var options = new VoiceNextConfiguration();

            configure?.Invoke(options);

            var discord = provider.GetRequiredService<IDiscordService>().Client;

            discord.UseVoiceNext(options);

            //
            // This is intentional; we don't need this "service", just the execution flow ;)
            // 
            return new DiscordExtensionsConfiguration();
        });

        return services;
    }
}