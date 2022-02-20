// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Types;

public static class GenericTypeExtensions
{
    public static bool TypeImplementsGenericInterface(this Type type, Type genericInterface)
    {
        if (!genericInterface.IsGenericTypeDefinition)
        {
            
        }

        return type
            .GetTypeInfo()
            .ImplementedInterfaces
            .Any(IsGenericInterface(genericInterface));
    }
    
    public static Type GetImplementedGenericInterfaceType(this Type type, Type genericInterface)
    {
        if (!TypeImplementsGenericInterface(type, genericInterface))
        {
            
        }

        return type
            .GetTypeInfo()
            .ImplementedInterfaces
            .Single(IsGenericInterface(genericInterface))
            .GetGenericArguments()[0];
    }

    static Func<Type, bool> IsGenericInterface(Type genericInterface)
        => type => type.IsGenericType && type.GetGenericTypeDefinition() == genericInterface;
}
