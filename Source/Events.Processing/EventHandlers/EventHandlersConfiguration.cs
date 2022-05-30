// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents the configuration of Event Store backwards compatibility.
/// </summary>
/// <param name="Fast">Whether to use the new fast event handlers.</param>
[Configuration("processing:eventhandlers")]
public record EventHandlersConfiguration(bool Fast = false)
{
    /// <summary>
    /// Whether to register a filter stream processor that keeps stream processor state and persists to a stream collection
    /// </summary>
    public bool ImplicitFilter => false;
}
