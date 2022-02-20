// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Types;

public static class TypeFilteringExtensions
{
    public static IEnumerable<Type> IgnoreClassesWithAttribute<TAttribute>(this IEnumerable<Type> classes)
        where TAttribute : Attribute
        => classes.Where(type => !Attribute.IsDefined(type, typeof(TAttribute)));

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
