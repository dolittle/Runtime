// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Base attribute to use on implemented gRPC services to indicate that they should be registered in a DI container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public abstract class ServiceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceAttribute"/> class.
    /// </summary>
    /// <param name="visibility">The visibility of the service with this attribute defined.</param>
    protected ServiceAttribute(EndpointVisibility visibility)
    {
        Visibility = visibility;
    }
    
    /// <summary>
    /// Gets the <see cref="EndpointVisibility"/> for the service with this attribute defined.
    /// </summary>
    public EndpointVisibility Visibility { get; }
}
