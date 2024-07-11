using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using BindingFlags = System.Reflection.BindingFlags;

namespace MikyM.Discord.Tests;

public class DiscordEventDispatcherTestsFixture
{
    public Mock<IServiceProvider> ServiceProvider { get; } = new();
    public Mock<ILogger<DiscordEventDispatcher>> Logger { get; } = new();
}

[CollectionDefinition("DiscordEventDispatcherTests")]
public class DiscordEventDispatcherTests : ICollectionFixture<DiscordEventDispatcherTestsFixture>
{
    [Collection("DiscordEventDispatcherTests")]
    public class ConstructorShould
    {
        private DiscordEventDispatcherTestsFixture _fixture;
        
        public ConstructorShould(DiscordEventDispatcherTestsFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void CorrectlyBuildDelegates()
        {
            var provider = MetadataProvider.Create();
            provider.AppendTypes(typeof(CommandExecutedEventArgsSubscriberImpl).Assembly.GetTypes());
            

            var func = () => new DiscordEventDispatcher(_fixture.ServiceProvider.Object, _fixture.Logger.Object,
                provider);
            
            func.Should().NotThrow().Subject.Should().NotBeNull();
        }
    }
    
    public class DispatchSingleAsyncShould
    {
        
        [Collection("DiscordEventDispatcherTests")]
        public class BasicOverload
        {
            private DiscordEventDispatcherTestsFixture _fixture;
        
            public BasicOverload(DiscordEventDispatcherTestsFixture fixture)
            {
                _fixture = fixture;
            }
        
            [Theory]
            [InlineData(null)]
            [InlineData(ResolveStrategy.KeyedInterface)]
            [InlineData(ResolveStrategy.Implementation)]
            public async Task ThrowNoException(ResolveStrategy? resolveStrategy)
            {
                // Arrange

                var services = new ServiceCollection();
            
                var type = resolveStrategy switch
                {
                    ResolveStrategy.KeyedInterface => typeof(SessionCreatedEventArgsSubscriberKeyedInterface),
                    ResolveStrategy.Implementation => typeof(SessionCreatedEventArgsSubscriberImplStr),
                    _ => typeof(SessionCreatedEventArgsSubscriberNone)
                };

                services.AddDiscordEventSubscriber(type);
            
                var serviceProvider = services.BuildServiceProvider();
            
                var meta = MetadataProvider.Create();
                meta.AppendTypes(new []{ type });
                var subject = new DiscordEventDispatcher(serviceProvider, _fixture.Logger.Object, meta);

                var builder = DiscordClientBuilder.CreateDefault("", DiscordIntents.AllUnprivileged, services);

                var client = builder.Build();

                var ctor = typeof(SessionCreatedEventArgs).GetConstructor(BindingFlags.CreateInstance |
                                                                          BindingFlags.NonPublic, Type.EmptyTypes);

                var args = (SessionCreatedEventArgs)ctor?.Invoke([])!;
            
                // Act && Assert
                var func = async () => await subject.DispatchSingleAsync(type, typeof(SessionCreatedEventArgs), client, args);
            
                await func.Should().NotThrowAsync();
            }
        }
        
        [Collection("DiscordEventDispatcherTests")]
        public class CommandOverload
        {
            private DiscordEventDispatcherTestsFixture _fixture;
        
            public CommandOverload(DiscordEventDispatcherTestsFixture fixture)
            {
                _fixture = fixture;
            }
        
            [Theory]
            [InlineData(null)]
            [InlineData(ResolveStrategy.KeyedInterface)]
            [InlineData(ResolveStrategy.Implementation)]
            public async Task ThrowNoException(ResolveStrategy? resolveStrategy)
            {
                // Arrange

                var services = new ServiceCollection();
            
                var type = resolveStrategy switch
                {
                    ResolveStrategy.KeyedInterface => typeof(CommandExecutedEventArgsSubscriberImpl),
                    ResolveStrategy.Implementation => typeof(CommandExecutedEventArgsSubscriberKeyedInterface),
                    _ => typeof(CommandExecutedEventArgsSubscriberNone)
                };

                services.AddDiscordEventSubscriber(type);
            
                var serviceProvider = services.BuildServiceProvider();
            
                var meta = MetadataProvider.Create();
                meta.AppendTypes(new []{ type });
            
                var subject = new DiscordEventDispatcher(serviceProvider, _fixture.Logger.Object, meta);

                var builder = DiscordClientBuilder.CreateDefault("", DiscordIntents.AllUnprivileged, services);

                var client = builder.Build();

                var ext = client.UseCommands();
            
                var args = new CommandExecutedEventArgs()
                {
                    CommandObject = null,
                    Context = null!,
                };
            
                // Act && Assert
                var func = async () => await subject.DispatchSingleAsync(type, typeof(CommandExecutedEventArgs), ext, args);
            
                await func.Should().NotThrowAsync();
            }
        }
    }
    
    [Collection("DiscordEventDispatcherTests")]
    public class DispatchSequentialOrderedPipeAsyncShould
    {
        private DiscordEventDispatcherTestsFixture _fixture;
        
        public DispatchSequentialOrderedPipeAsyncShould(DiscordEventDispatcherTestsFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async Task ThrowNoException()
        {
            // Arrange

            var services = new ServiceCollection();

            services.AddDiscordEventSubscriber(typeof(CommandExecutedEventArgsSubscriberNone));
            
            var serviceProvider = services.BuildServiceProvider();
            
            var meta = MetadataProvider.Create();
            meta.AppendTypes(new []{ typeof(CommandExecutedEventArgsSubscriberNone) });
            
            var subject = new DiscordEventDispatcher(serviceProvider, _fixture.Logger.Object, meta);

            var builder = DiscordClientBuilder.CreateDefault("", DiscordIntents.AllUnprivileged, services);

            var client = builder.Build();

            var ext = client.UseCommands();
            
            var args = new CommandExecutedEventArgs()
            {
                CommandObject = null,
                Context = null!,
            };
            
            // Act && Assert
            var func = async () => await subject.DispatchSequentialOrderedPipeAsync(typeof(CommandExecutedEventArgs), ext, args);
            
            await func.Should().NotThrowAsync();
        }
    }
    
    [Collection("DiscordEventDispatcherTests")]
    public class DispatchParallelPipeAsyncShould
    {
        private DiscordEventDispatcherTestsFixture _fixture;

        public DispatchParallelPipeAsyncShould(DiscordEventDispatcherTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ThrowNoException()
        {
            // Arrange

            var services = new ServiceCollection();

            services.AddDiscordEventSubscriber(typeof(SessionCreatedEventArgsSubscriberNone));

            var serviceProvider = services.BuildServiceProvider();

            var meta = MetadataProvider.Create();
            meta.AppendTypes(new []{ typeof(SessionCreatedEventArgsSubscriberNone) });
            
            var subject = new DiscordEventDispatcher(serviceProvider, _fixture.Logger.Object, meta);

            var builder = DiscordClientBuilder.CreateDefault("", DiscordIntents.AllUnprivileged, services);

            var client = builder.Build();

            var ctor = typeof(SessionCreatedEventArgs).GetConstructor(BindingFlags.CreateInstance |
                                                                      BindingFlags.NonPublic, Type.EmptyTypes);

            var args = (SessionCreatedEventArgs)ctor?.Invoke([])!;
            
            // Act && Assert
            var func = async () => await subject.DispatchParallelPipeAsync(typeof(SessionCreatedEventArgs), client, args);
            
            await func.Should().NotThrowAsync();
        }
    }
}
