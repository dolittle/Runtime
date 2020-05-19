// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates.for_AggregateRoot
{
    public class when_creating_aggregate_root
    {
        static EventSourceId event_source;
        static ArtifactId aggregate_type;
        static AggregateRootVersion version;
        static AggregateRoot aggregate_root;

        Establish context = () =>
        {
            event_source = Guid.NewGuid();
            aggregate_type = Guid.NewGuid();
            version = random.aggregate_root_version;
        };

        Because of = () => aggregate_root = new AggregateRoot(event_source, aggregate_type, version);

        It should_have_the_correct_event_source = () => aggregate_root.EventSource.ShouldEqual(event_source.Value);
        It should_have_the_correct_aggregate_type = () => aggregate_root.AggregateType.ShouldEqual(aggregate_type.Value);
        It should_have_the_correct_version = () => aggregate_root.Version.ShouldEqual(version.Value);
    }
}