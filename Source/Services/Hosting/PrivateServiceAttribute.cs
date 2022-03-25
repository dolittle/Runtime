// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Indicates that the class should be registered as a private gRPC service in a DI container.
/// </summary>
public class PrivateServiceAttribute : ServiceAttribute 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateServiceAttribute"/> class.
    /// </summary>
    public PrivateServiceAttribute()
        : base(EndpointVisibility.Private)
    {
    }
}
