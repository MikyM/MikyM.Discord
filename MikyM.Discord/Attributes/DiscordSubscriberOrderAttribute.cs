using System;

namespace MikyM.Discord.Attributes;

/// <summary>
/// Represents an attribute that indicates the order of the subscriber within the dispatch pipe.
/// </summary>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DiscordSubscriberOrderAttribute : Attribute
{
    /// <summary>
    /// The order of the subscriber.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DiscordSubscriberOrderAttribute"/>.
    /// </summary>
    /// <param name="order">The order of the subscriber.</param>
    public DiscordSubscriberOrderAttribute(int order)
    {
        Order = order;
    }   
}
