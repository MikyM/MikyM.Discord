namespace MikyM.Discord;

/// <summary>
/// The dispatch strategy.
/// </summary>
[PublicAPI]
public enum DispatchStrategy
{
    /// <summary>
    /// Indicates that the subscribers will be called sequentially, according to order if any.
    /// </summary>
    Sequential,
    /// <summary>
    /// Indicates that the subscribers will be called in parallel.
    /// </summary>
    Parallel
}
