// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion.Types;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion.Attributes;

/// <summary>
/// Represents an implementation of <see cref="ICanAddTenantServices"/> for tenant services.
/// </summary>
/// <typeparam name="TAttribute">The <see cref="Type"/> of the attribute.</typeparam>
[DisableAutoRegistration]
public class TenantServicesBuilderForTypesWith<TAttribute> : ICanAddTenantServices
    where TAttribute : Attribute
{
    readonly ICanAddTenantServicesForTypesWith<TAttribute> _builder;
    readonly Dictionary<Type, IEnumerable<TAttribute>> _typesWithAttribute = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantServicesBuilderForTypesWith{TAttribute}"/> class.
    /// </summary>
    /// <param name="builder">The <see cref="ICanAddTenantServicesForTypesWith{TAttribute}"/>.</param>
    /// <param name="discoveredClasses">The discovered classes.</param>
    public TenantServicesBuilderForTypesWith(ICanAddTenantServicesForTypesWith<TAttribute> builder, IEnumerable<Type> discoveredClasses)
    {
        _builder = builder;
        foreach (var type in discoveredClasses)
        {
            var attributes = type.GetAttributes<TAttribute>();
            if (attributes.Any())
            {
                _typesWithAttribute.Add(type, attributes);
            }
        }
    }

    /// <inheritdoc />
    public void AddFor(TenantId tenant, IServiceCollection services)
    {
        foreach (var (type, attributes) in _typesWithAttribute)
        {
            foreach (var attribute in attributes)
            {
                _builder.AddServiceFor(type, attribute, tenant, services);
            }
        }
    }
}
