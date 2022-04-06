// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;
using MongoDB.Driver;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence.for_UpdateAggregateVersionsAfterCommit;

public class and_updating_a_version_crashes : given.all_dependencies
{
    static ArtifactId aggregate_root;
    static EventSourceId event_source;
    static AggregateRootVersion expected_version;
    static Exception exception;
    static Exception catched_exception;
    Establish context = () =>
    {
        aggregate_root = "abe7ff8b-697f-4566-aace-9b3b94f6facd";
        event_source = "some source";
        expected_version = 2;
        exception = new Exception("some failure");
        aggregate_roots
            .Setup(_ => _.IncrementVersionFor(Moq.It.IsAny<IClientSessionHandle>(), event_source, aggregate_root, expected_version, expected_version + 1, cancellation_token))
            .Throws(exception);
    };
    
    Because of = () => catched_exception = Catch.Exception(() => updater.UpdateAggregateVersions(session.Object, create_commit(0, (aggregate_root, event_source, expected_version, 1)), cancellation_token).GetAwaiter().GetResult());

    It should_update_aggregate_version = () => verify_updated_aggregate_root_version_for(Times.Once(), (event_source, aggregate_root), (expected_version, expected_version + 1));
    It should_update_no_other_versions = verify_no_more_updates;
    It should_catch_the_exception = () => catched_exception.ShouldEqual(exception);
}