// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.DependencyInversion.Types;

namespace Dolittle.Runtime.DependencyInversion.Attributes;

/// <summary>
/// Represents a static class for creating <see cref="ICanAddServices"/> for non-tenant services.
/// </summary>
public static class ServicesForTypesWith
{
    /// <summary>
    /// Creates an instance of <see cref="ICanAddTenantServices"/> by reflection for the given <see cref="Type"/> that can add tenant services for types with an attribute.
    /// </summary>
    /// <param name="adderType">The <see cref="Type"/> of the tenant services adder.</param>
    /// <param name="discoveredClasses">The discovered classes implementing </param>
    /// <returns>The <see cref="ICanAddTenantServices"/> that can add the tenant services .</returns>
    public static ICanAddServices CreateBuilderFor(Type adderType, IEnumerable<Type> discoveredClasses)
    {
        var attributeType = adderType.GetImplementedGenericInterfaceGenericType(typeof(ICanAddServicesForTypesWith<>));
        var builderType = typeof(ServicesBuilderForTypesWith<>).MakeGenericType(attributeType);

        var adder = Activator.CreateInstance(adderType);
        if (adder == default)
        {
            throw new CouldNotCreateInstanceOfType(adderType);
        }

        var builder = Activator.CreateInstance(builderType, adder, discoveredClasses);
        if (builder is not ICanAddServices typedBuilder)
        {
            throw new CouldNotCreateInstanceOfType(builderType);
        }

        return typedBuilder;
    }
}
