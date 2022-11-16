// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence.for_UpdateAggregateVersionsAfterCommit.when_commit_has.two_commited_aggregate_events;

public class for_two_aggregate_instance : given.all_dependencies
{
    static ArtifactId aggregate_root_1;
    static EventSourceId event_source_1;
    static AggregateRootVersion expected_version_1;
    
    static ArtifactId aggregate_root_2;
    static EventSourceId event_source_2;
    static AggregateRootVersion expected_version_2;

    Establish context = () =>
    {
        aggregate_root_1 = "abe7ff8b-697f-4566-aace-9b3b94f6facd";
        event_source_1 = "some source";
        expected_version_1 = 2;
        
        aggregate_root_2 = "ef1b627b-9fe7-4bef-a239-3b89fcb5ba32";
        event_source_2 = "some other source";
        expected_version_2 = 5;
    };
    
    Because of = () => updater.UpdateAggregateVersions(
        session.Object,
        create_commit(
            0,
            (aggregate_root_1, event_source_1, expected_version_1, 2),
            (aggregate_root_2, event_source_2, expected_version_2, 2)),
        cancellation_token).GetAwaiter().GetResult();

    It should_update_first_aggregate_version = () => verify_updated_aggregate_root_version_for(Times.Once(), (event_source_1, aggregate_root_1), (expected_version_1, expected_version_1 + 2));
    It should_update_second_aggregate_version = () => verify_updated_aggregate_root_version_for(Times.Once(), (event_source_2, aggregate_root_2), (expected_version_2, expected_version_2 + 2));
    It should_update_no_other_versions = verify_no_more_updates;
}