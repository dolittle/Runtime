// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence.for_UpdateAggregateVersionsAfterCommit.when_commit_has.two_commited_aggregate_events;

public class for_one_aggregate_instance : given.all_dependencies
{
    static ArtifactId aggregate_root;
    static EventSourceId event_source;
    static AggregateRootVersion expected_version;

    Establish context = () =>
    {
        aggregate_root = "abe7ff8b-697f-4566-aace-9b3b94f6facd";
        event_source = "some source";
        expected_version = 2;
    };
    
    Because of = () => updater.UpdateAggregateVersions(session.Object, create_commit(0, (aggregate_root, event_source, expected_version, 2)), cancellation_token).GetAwaiter().GetResult();

    It should_update_aggregate_version = () => verify_updated_aggregate_root_version_for(Times.Once(), (event_source, aggregate_root), (expected_version, expected_version + 2));
    It should_update_no_other_versions = verify_no_more_updates;
}