// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Configuration;

/// <summary>
/// Exception that gets thrown when there are multiple implementations of <see cref="ICanProvideDefaultConfigurationFor{T}"/>
/// for a specific <see cref="IConfigurationObject"/>.
/// </summary>
public class MultipleDefaultConfigurationProvidersFoundForConfigurationObject : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleDefaultConfigurationProvidersFoundForConfigurationObject"/> class.
    /// </summary>
    /// <param name="type">Type of <see cref="IConfigurationObject"/> there are multiple default providers for.</param>
    public MultipleDefaultConfigurationProvidersFoundForConfigurationObject(Type type)
        : base($"There are multiple implementations of default value providers for configuration object of type '{type.AssemblyQualifiedName}'")
    {
    }
}