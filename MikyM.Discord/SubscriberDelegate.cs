using System.Threading.Tasks;
using DSharpPlus.AsyncEvents;

namespace MikyM.Discord;

internal delegate Task SubscriberDelegate(object subscriber, object sender, AsyncEventArgs args);
