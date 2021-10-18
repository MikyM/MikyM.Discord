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
using DSharpPlus.SlashCommands;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MikyM.Discord.Extensions.SlashCommands.Attributes;
using MikyM.Discord.Extensions.SlashCommands.Events;
using MikyM.Discord.Extensions.SlashCommands.Util;
using MikyM.Discord.Interfaces;
using MikyM.Discord.Util;
using OpenTracing;

namespace MikyM.Discord.Extensions.SlashCommands
{
    [UsedImplicitly]
    public static class DiscordServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds Interactivity extension to <see cref="IDiscordService" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" />.</param>
        /// <param name="configuration">The <see cref="SlashCommandsConfiguration" />.</param>
        /// <param name="extension">The <see cref="SlashCommandsExtension" />.</param>
        /// <param name="autoRegisterSubscribers">
        ///     If true, classes with subscriber attributes will get registered as event
        ///     subscribers automatically. This is the default.
        /// </param>
        /// <returns>The <see cref="IServiceCollection" />.</returns>
        [UsedImplicitly]
        public static IServiceCollection AddDiscordSlashCommands(
            this IServiceCollection services,
            Action<SlashCommandsConfiguration?> configuration = null,
            Action<SlashCommandsExtension?> extension = null,
            bool autoRegisterSubscribers = true
        )
        {
            services.AddSingleton(typeof(IDiscordExtensionConfiguration), provider =>
            {
                var options = new SlashCommandsConfiguration();

                configuration?.Invoke(options);

                //
                // Make all services available to bot commands
                // 
                options.Services = provider;

                var discord = provider.GetRequiredService<IDiscordService>().Client;

                var ext = discord.UseSlashCommands(options);

                extension?.Invoke(ext);

                ext.ContextMenuErrored += async (sender, args) =>
                {
                    using var workScope = provider.GetRequiredService<ITracer>()
                        .BuildSpan(nameof(ext.ContextMenuErrored))
                        .IgnoreActiveSpan()
                        .StartActive(true);
                    workScope.Span.SetTag("Context.CommandName", args.Context.CommandName);

                    using var scope = provider.CreateScope();

                    foreach (var eventsSubscriber in scope.GetDiscordSlashCommandsEventsSubscriber())
                        await eventsSubscriber.SlashCommandsOnContextMenuErrored(sender, args);
                };

                ext.ContextMenuExecuted +=
                    async (sender, args) =>
                    {
                        using var workScope = provider.GetRequiredService<ITracer>()
                            .BuildSpan(nameof(ext.ContextMenuExecuted))
                            .IgnoreActiveSpan()
                            .StartActive(true);
                        workScope.Span.SetTag("Context.CommandName", args.Context.CommandName);

                        using var scope = provider.CreateScope();

                        foreach (var eventsSubscriber in scope.GetDiscordSlashCommandsEventsSubscriber())
                            await eventsSubscriber.SlashCommandsOnContextMenuExecuted(sender, args);
                    };

                ext.SlashCommandErrored +=
                    async (sender, args) =>
                    {
                        using var workScope = provider.GetRequiredService<ITracer>()
                            .BuildSpan(nameof(ext.SlashCommandErrored))
                            .IgnoreActiveSpan()
                            .StartActive(true);
                        workScope.Span.SetTag("Context.CommandName", args.Context.CommandName);

                        using var scope = provider.CreateScope();

                        foreach (var eventsSubscriber in scope.GetDiscordSlashCommandsEventsSubscriber())
                            await eventsSubscriber.SlashCommandsOnSlashCommandErrored(sender, args);
                    };

                ext.SlashCommandExecuted +=
                    async (sender, args) =>
                    {
                        using var workScope = provider.GetRequiredService<ITracer>()
                            .BuildSpan(nameof(ext.SlashCommandExecuted))
                            .IgnoreActiveSpan()
                            .StartActive(true);
                        workScope.Span.SetTag("Context.CommandName", args.Context.CommandName);

                        using var scope = provider.CreateScope();

                        foreach (var eventsSubscriber in scope.GetDiscordSlashCommandsEventsSubscriber())
                            await eventsSubscriber.SlashCommandsOnSlashCommandExecuted(sender, args);
                    };

                //
                // This is intentional; we don't need this "service", just the execution flow ;)
                // 
                return new DiscordExtensionsConfiguration();
            });

            if (!autoRegisterSubscribers)
                return services;

            foreach (var type in AssemblyTypeHelper.GetTypesWith<DiscordSlashCommandsEventsSubscriberAttribute>())
                services.AddDiscordSlashCommandsEventsSubscriber(type);

            return services;
        }

        #region Subscribers

        [UsedImplicitly]
        public static IServiceCollection AddDiscordSlashCommandsEventsSubscriber<T>(this IServiceCollection services)
            where T : IDiscordSlashCommandsEventsSubscriber
        {
            return services.AddDiscordSlashCommandsEventsSubscriber(typeof(T));
        }

        public static IServiceCollection AddDiscordSlashCommandsEventsSubscriber(this IServiceCollection services,
            Type t)
        {
            return services.AddScoped(typeof(IDiscordSlashCommandsEventsSubscriber), t);
        }

        #endregion
    }
}