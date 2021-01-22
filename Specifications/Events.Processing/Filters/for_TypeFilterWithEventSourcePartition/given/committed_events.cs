// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartition.given
{
    public static class committed_events
    {
        public static CommittedEvent single(EventSourceId event_source) => single_with_artifact(event_source, artifacts.single());

        public static CommittedEvent single_with_artifact(EventSourceId event_source, Artifact artifact) =>
            new CommittedEvent(
                0,
                DateTimeOffset.Now,
                event_source,
                execution_contexts.create(),
                artifact,
                false,
                "");
    }
}