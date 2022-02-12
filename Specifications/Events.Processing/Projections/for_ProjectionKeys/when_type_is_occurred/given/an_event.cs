// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ProjectionKeys.when_type_is_occurred.given;

public static class an_event
{
    public static CommittedEvent that_occurred_at(DateTimeOffset occurred)
        => new(0, occurred, "event source", execution_contexts.create(), Artifact.New(), false, "{\"hello\": \"world\"}");
}