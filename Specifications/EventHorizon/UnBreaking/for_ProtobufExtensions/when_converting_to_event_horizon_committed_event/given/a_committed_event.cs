// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_to_event_horizon_committed_event.given;

public class a_committed_event
{
    public static CommittedEvent with_event_source(EventSourceId event_source)
        => new(
            0,
            DateTimeOffset.Now,
            event_source,
            new ExecutionContext(
                "9dcaad5a-cf10-4ee1-96cd-68b32471b8ea",
                "c33eadcd-c52b-48ec-80ae-c1537bf3755a",
                Version.NotSet, 
                Environment.Development,
                "141e5e37-4d58-4e23-a69b-124eb7b5b9de",
                ActivitySpanId.CreateRandom(),
                Claims.Empty,
                CultureInfo.InvariantCulture),
            Artifact.New(),
            true,
            "");
}