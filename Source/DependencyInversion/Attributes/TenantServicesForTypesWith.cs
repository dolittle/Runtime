// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.DependencyInversion.Types;

namespace Dolittle.Runtime.DependencyInversion.Attributes;

public static class TenantServicesForTypesWith
{
    public static ICanAddTenantServices CreateBuilderFor(Type adderType, IEnumerable<Type> discoveredClasses)
    {
        var attributeType = adderType.GetImplementedGenericInterfaceType(typeof(ICanAddTenantServicesForTypesWith<>));
        var builderType = typeof(ICanAddTenantServicesForTypesWith<>).MakeGenericType(attributeType);

        var adder = Activator.CreateInstance(adderType);
        if (adder == default)
        {
            throw new CouldNotCreateInstanceOfType(adderType);
        }

        var builder = Activator.CreateInstance(builderType, adder, discoveredClasses);
        if (builder is not ICanAddTenantServices typedBuilder)
        {
            throw new CouldNotCreateInstanceOfType(builderType);
        }

        return typedBuilder;
    }
}
