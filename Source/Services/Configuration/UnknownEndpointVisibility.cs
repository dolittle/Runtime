// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Configuration;

/// <summary>
/// Exception that gets thrown when attempting to get the configuration for an unknown <see cref="EndpointVisibility"/>.
/// </summary>
public class UnknownEndpointVisibility : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownEndpointVisibility"/> class.
    /// </summary>
    /// <param name="visibility">The visibility that is unknown.</param>
    public UnknownEndpointVisibility(EndpointVisibility visibility)
        : base($"The {nameof(EndpointVisibility)} {visibility} is unknown")
    {
    }
}
