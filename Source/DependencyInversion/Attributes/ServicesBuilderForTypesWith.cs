// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.DependencyInversion.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion.Attributes;

/// <summary>
/// Represents an implementation of <see cref="ICanAddServices"/> for non-tenant services.
/// </summary>
/// <typeparam name="TAttribute">The <see cref="Type"/> of the attribute.</typeparam>
[DisableAutoRegistration]
public class ServicesBuilderForTypesWith<TAttribute> : ICanAddServices
    where TAttribute : Attribute
{
    readonly ICanAddServicesForTypesWith<TAttribute> _builder;
    readonly Dictionary<Type, IEnumerable<TAttribute>> _typesWithAttribute = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesBuilderForTypesWith{TAttribute}"/> class.
    /// </summary>
    /// <param name="builder">The <see cref="ICanAddServicesForTypesWith{TAttribute}"/>.</param>
    /// <param name="discoveredClasses">The discovered classes.</param>
    public ServicesBuilderForTypesWith(ICanAddServicesForTypesWith<TAttribute> builder, IEnumerable<Type> discoveredClasses)
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
    public void AddTo(IServiceCollection services)
    {
        foreach (var (type, attributes) in _typesWithAttribute)
        {
            foreach (var attribute in attributes)
            {
                _builder.AddServiceFor(type, attribute, services);
            }
        }
    }
}
