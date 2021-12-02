// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Exception that gets thrown when trying to find a <see cref="ResourceType"/> mapping to a specific <see cref="Type"/>.
/// </summary>
public class NoResourceTypeMatchingConfigurationType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoResourceTypeMatchingConfigurationType"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> of configuration.</param>
    public NoResourceTypeMatchingConfigurationType(Type type)
        : base($"Could not map the Type {type.FullName} up to a {typeof(ResourceType).FullName}")
    {
    }
}