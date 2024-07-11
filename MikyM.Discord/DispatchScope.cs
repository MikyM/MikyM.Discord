namespace MikyM.Discord;

/// <summary>
/// The dispatch scope.
/// </summary>
[PublicAPI]
public enum DispatchScope
{
    /// <summary>
    /// Indicates that the scope should be per subscriber.
    /// </summary>
    PerSubscriber,
    /// <summary>
    /// Indicates that the scope should be per event (shared between subscribers).
    /// </summary>
    PerEvent
}
