// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    public record EventHandlerRegistrationArguments(
        ExecutionContext ExecutionContext,
        EventProcessorId EventHandler,
        EventHandlerAlias Alias,
        IEnumerable<ArtifactId> EventTypes,
        bool Partitioned,
        ScopeId Scope);
}
