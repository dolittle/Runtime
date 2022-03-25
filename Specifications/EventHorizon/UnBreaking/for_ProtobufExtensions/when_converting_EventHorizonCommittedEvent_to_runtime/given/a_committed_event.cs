// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Artifacts.Contracts;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_EventHorizonCommittedEvent_to_runtime.given;

public class a_committed_event
{
    protected static Contracts.EventHorizonCommittedEvent committed_event;

    Establish context = () =>
    {
        committed_event = new EventHorizonCommittedEvent
        {
            EventLogSequenceNumber = 0,
            Occurred = DateTimeOffset.Now.ToTimestamp(),
            ExecutionContext = new Execution.ExecutionContext(
                "9dcaad5a-cf10-4ee1-96cd-68b32471b8ea",
                "c33eadcd-c52b-48ec-80ae-c1537bf3755a",
                Version.NotSet, 
                Environment.Development,
                "141e5e37-4d58-4e23-a69b-124eb7b5b9de",
                Claims.Empty,
                CultureInfo.InvariantCulture).ToProtobuf(),
            EventSourceId = "an event source",
            Content = "",
            Public = true,
            EventType = new Artifact{Generation = 0, Id = new Guid("88d4d991-7873-425c-819b-dcb3c789eb88").ToProtobuf()},
            EventSourceIdLegacy = new Guid("3b8a0c51-56a4-4f85-ae4f-aec2e1b49abd").ToProtobuf()
        };
    };
}