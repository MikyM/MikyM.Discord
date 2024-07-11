using System;

namespace MikyM.Discord;

internal readonly struct SubscriberDelegateKey
{
    private readonly int _hash;

    private SubscriberDelegateKey(Type implementation, Type eventType)
    {
        _hash = HashCode.Combine(nameof(SubscriberDelegateKey), implementation, eventType);
    }
    
    internal static SubscriberDelegateKey Create(Type implementation, Type eventType)
    {
        return new SubscriberDelegateKey(implementation, eventType);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is SubscriberDelegateKey other && GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public static bool operator ==(SubscriberDelegateKey left, SubscriberDelegateKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SubscriberDelegateKey left, SubscriberDelegateKey right)
    {
        return !left.Equals(right);
    }
}
