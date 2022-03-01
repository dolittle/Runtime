// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Extensions on <see cref="IEnumerable{T}"/> of <see cref="Type"/> for type filtering.
/// </summary>
public static class TypeFilteringExtensions
{
    /// <summary>
    /// Ignore classes with a specific attribute.
    /// </summary>
    /// <param name="classes">The <see cref="IEnumerable{T}"/> classes to filter.</param>
    /// <typeparam name="TAttribute">The <see cref="Type"/> of the attribute.</typeparam>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Type"/> without the <typeparamref name="TAttribute"/> attribute.</returns>
    public static IEnumerable<Type> IgnoreClassesWithAttribute<TAttribute>(this IEnumerable<Type> classes)
        where TAttribute : Attribute
        => classes.Where(type => !Attribute.IsDefined(type, typeof(TAttribute)));
    
    /// <summary>
    /// Filters classes implementing a specific type.
    /// </summary>
    /// <param name="classes">The <see cref="IEnumerable{T}"/> classes to filter.</param>
    /// <param name="implementing">The <see cref="Type"/> that the classes should be filtered on.</param>
    /// <param name="classesImplementing">Outputted classes implementing the given <see cref="Type"/>.</param>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Type"/> not implementing the given <see cref="Type"/>.</returns>
    public static IEnumerable<Type> FilterClassesImplementing(this IEnumerable<Type> classes, Type implementing, out IEnumerable<Type> classesImplementing)
    {
        var predicate = CreateImplementingPredicate(implementing);
        var groupedByImplementing = classes.ToLookup(predicate);
        classesImplementing = groupedByImplementing[true];
        return groupedByImplementing[false];
    }

    static Func<Type, bool> CreateImplementingPredicate(Type implementing)
    {
        if (implementing.ContainsGenericParameters)
        {
            return type => type.GetTypeInfo().ImplementedInterfaces.Any(_ => _.IsGenericType && _.GetGenericTypeDefinition() == implementing);
        }

        return type => type.GetTypeInfo().ImplementedInterfaces.Any(_ => _ == implementing);
    }
}
