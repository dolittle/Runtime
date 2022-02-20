// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Represents the definition of an implemented gRPC service.
/// </summary>
public class ServiceDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDefinition"/> class.
    /// </summary>
    /// <param name="visibility">The visibility of the implemented service.</param>
    /// <param name="implementationType">The type that implements the service.</param>
    public ServiceDefinition(EndpointVisibility visibility, Type implementationType)
    {
        Visibility = visibility;
        ImplementationType = implementationType;
    }
    
    /// <summary>
    /// Gets the visibility of the implemented service.
    /// </summary>
    public EndpointVisibility Visibility { get; }
    
    /// <summary>
    /// Gets the type that implements the service.
    /// </summary>
    public Type ImplementationType { get; }
}
