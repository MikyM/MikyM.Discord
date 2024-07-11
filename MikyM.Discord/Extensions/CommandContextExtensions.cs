using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace MikyM.Discord.Extensions;

/// <summary>
/// Command context extensions.
/// </summary>
[PublicAPI]
public static class CommandContextExtensions
{
    /// <summary>Creates a deferred response to this interaction.</summary>
    /// <param name="context">The context.</param>
    /// <param name="ephemeral">If the response should be ephemeral.</param>
    public static Task DeferResponseAsync(this CommandContext context, bool ephemeral)
        => context is not SlashCommandContext slashContext 
            ? context.DeferResponseAsync().AsTask() 
            : slashContext.Interaction.DeferAsync(ephemeral);
}
