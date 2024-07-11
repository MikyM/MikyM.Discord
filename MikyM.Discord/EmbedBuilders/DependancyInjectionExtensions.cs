using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MikyM.Discord.EmbedBuilders.Builders;

namespace MikyM.Discord.EmbedBuilders;

/// <summary>
/// DI extensions for embed builders.
/// </summary>
[PublicAPI]
public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Registers <see cref="IEnhancedDiscordEmbedBuilder"/> with the <see cref="IServiceCollection"/>.
    /// <br></br><br></br>This method will also try to register other builders implementing <see cref="IEnhancedDiscordEmbedBuilder"/> with their concrete implementations by naming convention.
    /// </summary>
    public static void AddEnhancedDiscordEmbedBuilders(this IServiceCollection services)
    {
        var pairs = typeof(IEnhancedDiscordEmbedBuilder).GetInterfaceImplementationPairs();

        if (pairs.Count == 0) return;

        foreach (var (intr, impl) in pairs)
        {
            if (impl is null) continue;
            services.TryAddTransient(intr, impl);
        }
    }

    /// <summary>
    /// Registers <see cref="IEnrichedDiscordEmbedBuilder"/> with the <see cref="IServiceCollection"/>.
    /// <br></br><br></br>This method will also try to register other builders implementing <see cref="IEnrichedDiscordEmbedBuilder"/> with their concrete implementations by naming convention.
    /// <br></br><br></br><see cref="AddEnhancedDiscordEmbedBuilders"/> will also be automatically called.
    /// </summary>
    public static void AddEnrichedDiscordEmbedBuilders(this IServiceCollection services)
    {
        AddEnhancedDiscordEmbedBuilders(services);

        var pairs = typeof(IEnrichedDiscordEmbedBuilder).GetInterfaceImplementationPairs();

        if (pairs.Count == 0) return;


        foreach (var (intr, impl) in pairs)
        {
            if (impl is null) continue;
            services.TryAddTransient(intr, impl);
        }
    }
    
    /// <summary>
    /// Checks whether the given type is assignable to another type supporting generic types.
    /// </summary>
    /// <param name="givenType">Type to check.</param>
    /// <param name="genericType">Type to compare with.</param>
    /// <returns>True if the given type is assignable to another type, otherwise false.</returns>
    private static bool IsAssignableToWithGenerics(this Type givenType, Type genericType)
    {
        if (!genericType.IsGenericType)
            return givenType.IsAssignableTo(genericType);

        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            return true;
        
        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type? baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToWithGenerics(baseType, genericType);
    }

    
    /// <summary>
    /// Gets a dictionary with interface implementation pairs that implement a given base interface.
    /// </summary>
    /// <param name="interfaceToSearchFor">Base interface to search for.</param>
    private static Dictionary<Type, Type?> GetInterfaceImplementationPairs(this Type interfaceToSearchFor)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var dict = assemblies
            .SelectMany(x => x.GetTypes()
                .Where(t => interfaceToSearchFor.IsDirectAncestor(t) &&
                            t.IsInterface))
            .ToDictionary(intr => intr,
                intr => assemblies.SelectMany(impl => impl.GetTypes())
                    .FirstOrDefault(impl =>
                        impl.IsAssignableToWithGenerics(intr) && impl.IsClass &&
                        intr.IsDirectAncestor(impl)));

        return dict;
    }
    
    /// <summary>
    /// Check if a type is a direct ancestor of given type
    /// </summary>
    private static bool IsDirectAncestor(this Type ancestorCandidate, Type type)
        => type.GetTypeInheritance().IsDirectAncestor(ancestorCandidate);
    
    /// <summary>
    /// Retrieves the type inheritance tree
    /// </summary>
    /// <param name="type">The type to find tree for.</param>
    /// <returns>The inheritance tree.</returns>
    private static InheritanceTree GetTypeInheritance(this Type type)
    {
        //get all the interfaces for this type
        var interfaces = type.GetInterfaces();

        //get all the interfaces for the ancestor interfaces
        var baseInterfaces = interfaces.SelectMany(i => i.GetInterfaces());

        //filter based on only the direct interfaces
        var directInterfaces = interfaces.Where(i => baseInterfaces.All(b => b != i));

        return new InheritanceTree(type, directInterfaces.Select(GetTypeInheritance).ToList());
    }
    
    /// <summary>
    /// Inheritance tree
    /// </summary>
    private class InheritanceTree
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ancestors"></param>
        public InheritanceTree(Type node, List<InheritanceTree> ancestors)
        {
            Node = node;
            Ancestors = ancestors;
        }

        /// <summary>
        /// Current type node.
        /// </summary>
        public Type Node { get; set; }
        /// <summary>
        /// Ancestors.
        /// </summary>
        public List<InheritanceTree> Ancestors { get; set; }

        /// <summary>
        /// Checks whether the given type is a direct ancestor.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the given type is a direct ancestor, otherwise false.</returns>
        public bool IsDirectAncestor(Type type)
            => Ancestors.Any(x =>
                (x.Node.IsGenericType ? x.Node.GetGenericTypeDefinition() : x.Node) ==
                (type.IsGenericType ? type.GetGenericTypeDefinition() : type));
    }
}
