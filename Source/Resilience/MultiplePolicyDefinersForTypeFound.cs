// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Exception that gets thrown whenthere are multiple implementations of <see cref="IDefinePolicyForType"/> in the system.
/// </summary>
public class MultiplePolicyDefinersForTypeFound : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiplePolicyDefinersForTypeFound"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> missing policy for.</param>
    public MultiplePolicyDefinersForTypeFound(Type type)
        : base($"Multiple implementations of IDefinePolicyForType found for '{type.AssemblyQualifiedName}' - there can be only one")
    {
    }
}