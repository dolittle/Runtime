// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store.Streams;
namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    public record EventHandlerStatus(
        EventHandlerId Id,
        IEnumerable<Artifact> EventTypes,
        bool Partitioned,
        EventHandlerAlias Alias,
        IDictionary<TenantId, IStreamProcessorState> States);
}