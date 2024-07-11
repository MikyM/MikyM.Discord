namespace MikyM.Discord;

/// <summary>
/// The event dispatch configuration.
/// </summary>
[PublicAPI]
public class DiscordEventDispatchConfiguration
{
    /// <summary>
    /// Gets or sets the maximum degree of parallelism for basic event dispatching.
    /// </summary>
    public int? MaxDegreeOfBasicEventParallelism { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum degree of parallelism for command event dispatching.
    /// </summary>
    public int? MaxDegreeOfCommandEventParallelism { get; set; }
    
    /// <summary>
    /// Gets or sets  the dispatch scope for basic events.
    /// </summary>
    public DispatchScope BasicDispatchScope { get; set; } = DispatchScope.PerSubscriber;
    
    /// <summary>
    /// Gets or sets  the dispatch scope for command events.
    /// </summary>
    public DispatchScope CommandDispatchScope { get; set; } = DispatchScope.PerSubscriber;
    
    /// <summary>
    /// Gets or sets  the dispatch strategy for basic events.
    /// </summary>
    public DispatchStrategy BasicDispatchStrategy { get; set; } = DispatchStrategy.Parallel;
    
    /// <summary>
    /// Gets or sets  the dispatch strategy for command events.
    /// </summary>
    public DispatchStrategy CommandDispatchStrategy { get; set; } = DispatchStrategy.Sequential;
}
