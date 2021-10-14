// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents a unique identifier for an event handler.
    /// </summary>
    public record EventHandlerId(ScopeId Scope, EventProcessorId EventHandler);
}