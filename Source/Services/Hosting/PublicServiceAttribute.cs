// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services.Hosting;

/// <summary>
/// Indicates that the class should be registered as a public gRPC service in a DI container.
/// </summary>
public class PublicServiceAttribute : ServiceAttribute 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicServiceAttribute"/> class.
    /// </summary>
    public PublicServiceAttribute()
        : base(EndpointVisibility.Public)
    {
    }
}
