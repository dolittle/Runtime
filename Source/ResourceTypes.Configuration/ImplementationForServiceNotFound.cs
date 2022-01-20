// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.ResourceTypes.Configuration;

/// <summary>
/// Exception that gets thrown when no implementation is found for a service.
/// </summary>
public class ImplementationForServiceNotFound : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImplementationForServiceNotFound"/> class.
    /// </summary>
    /// <param name="service"><see cref="Type"/> of service.</param>
    public ImplementationForServiceNotFound(Type service)
        : base($"No implementation was found for the service {service.FullName}")
    {
    }
}