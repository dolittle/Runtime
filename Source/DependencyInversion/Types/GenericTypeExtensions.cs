// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.DependencyInversion.Types;

/// <summary>
/// Extensions on <see cref="Type"/> for getting information about generic types.
/// </summary>
public static class GenericTypeExtensions
{
    /// <summary>
    /// Tries to get the generic type of an implemented generic interface.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the implemented generic interface type from.</param>
    /// <param name="openGenericInterface">The open generic interface.</param>
    /// <returns>The generic type of an implemented generic interface.</returns>
    public static Type GetImplementedGenericInterfaceGenericType(this Type type, Type openGenericInterface)
    {
        var implementedInterfaces = type
            .GetTypeInfo()
            .ImplementedInterfaces
            .Where(IsGenericInterface(openGenericInterface)).ToArray();

        switch (implementedInterfaces.Length)
        {
            case 0:
                throw new TypeDoesNotImplementGenericInterface(type, openGenericInterface);
            case > 1:
                throw new TypeImplementsGenericInterfaceMultipleTimes(type, openGenericInterface);
        }

        var implementedInterface = implementedInterfaces[0];
        var implementedInterfaceGenericArguments = implementedInterface.GetGenericArguments();
        if (implementedInterfaceGenericArguments.Length > 1)
        {
            throw new OpenGenericInterfaceShouldHaveOnlyOneGenericParameter(type, implementedInterface);
        }

        return implementedInterfaceGenericArguments[0];
    }

    static Func<Type, bool> IsGenericInterface(Type genericInterface)
        => type => type.IsGenericType && type.GetGenericTypeDefinition() == genericInterface;
}
