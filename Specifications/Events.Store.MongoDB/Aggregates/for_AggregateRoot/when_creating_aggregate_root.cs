// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoot;

public class when_creating_aggregate_root
{
    static EventSourceId event_source;
    static ArtifactId aggregate_type;
    static AggregateRootVersion version;
    static AggregateRoot aggregate_root;

    Establish context = () =>
    {
        event_source = "a970569d-event-source-b608d25d38cb";
        aggregate_type = Guid.Parse("4c95bbef-ce78-4ffe-b1aa-36e7ad7fa6c5");
        version = random.aggregate_root_version;
    };

    Because of = () => aggregate_root = new AggregateRoot(event_source, aggregate_type, version);

    It should_have_the_correct_event_source = () => aggregate_root.EventSource.Should().Be(event_source.Value);
    It should_have_the_correct_aggregate_type = () => aggregate_root.AggregateType.Should().Be(aggregate_type.Value);
    It should_have_the_correct_version = () => aggregate_root.Version.Should().Be(version.Value);
}