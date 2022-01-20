// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.ResourceTypes;

/// <summary>
/// Defines a full representation of a <see cref="ResourceType"/> of a specific <see cref="ResourceTypeImplementation"/>.
/// </summary>
public interface IRepresentAResourceType
{
    /// <summary>
    /// Gets the <see cref="ResourceType"/> it represents.
    /// </summary>
    ResourceType Type { get; }

    /// <summary>
    /// Gets the <see cref="ResourceTypeImplementation"/> of the resource - identified typically in configuration files.
    /// </summary>
    ResourceTypeImplementation ImplementationName { get; }

    /// <summary>
    /// Gets the <see cref="System.Type"/> of the configuration object to expect and create proper bindings for.
    /// </summary>
    Type ConfigurationObjectType { get; }

    /// <summary>
    /// Gets the bindings, the service => implementation map.
    /// </summary>
    IDictionary<Type, Type> Bindings { get; }
}