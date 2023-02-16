// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Services.Configuration;

/// <summary>
/// Represents the configuration of reverse calls for gRPC services.
/// </summary>
[Configuration("reverseCalls")]
public record ReverseCallsConfiguration
{
    /// <summary>
    /// Gets the configuration for whether to use actors for reverse calls.
    /// </summary>
    public bool UseActors { get; init; }
    
}
