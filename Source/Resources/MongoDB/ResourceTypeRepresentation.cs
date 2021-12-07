// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes;

namespace Dolittle.Runtime.Resources.MongoDB;

/// <inheritdoc/>
public class ResourceTypeRepresentation : IRepresentAResourceType
{
    static readonly IDictionary<Type, Type> _bindings = new Dictionary<Type, Type>
    {
        { typeof(IKnowTheConnectionString), typeof(ConnectionStringFromResourceConfiguration) }
    };

    /// <inheritdoc/>
    public ResourceTypes.ResourceType Type => ResourceType.ResourceTypeName;

    /// <inheritdoc/>
    public ResourceTypeImplementation ImplementationName => "MongoDB";

    /// <inheritdoc/>
    public Type ConfigurationObjectType => typeof(ResourceConfiguration);

    /// <inheritdoc/>
    public IDictionary<Type, Type> Bindings => _bindings;
}