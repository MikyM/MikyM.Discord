using System;

namespace MikyM.Discord.Attributes;

/// <summary>
/// Represents an attribute that indicates by which service type should the subscriber be resolved.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DiscordSubscriberResolvedByAttribute : Attribute
{
    /// <summary>
    /// The resolve strategy.
    /// </summary>
    public ResolveStrategy ResolveStrategy { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DiscordSubscriberResolvedByAttribute"/>.
    /// </summary>
    /// <param name="resolveStrategy">The resolve strategy.</param>
    public DiscordSubscriberResolvedByAttribute(ResolveStrategy resolveStrategy)
    {
        ResolveStrategy = resolveStrategy;
    }
}
