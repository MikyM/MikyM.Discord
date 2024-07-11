namespace MikyM.Discord;

/// <summary>
/// Resolve strategy for resolving subscribers.
/// </summary>
[PublicAPI]
public enum ResolveStrategy
{
    /// <summary>
    /// Implementation of the subscriber.
    /// </summary>
    Implementation,
    /// <summary>
    /// The closed generic interface of the subscriber.
    /// </summary>
    KeyedInterface
}
