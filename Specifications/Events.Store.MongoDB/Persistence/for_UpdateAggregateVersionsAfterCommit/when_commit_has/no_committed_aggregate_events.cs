// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence.for_UpdateAggregateVersionsAfterCommit.when_commit_has;

public class no_committed_aggregate_events : given.all_dependencies
{
    Because of = () => updater.UpdateAggregateVersions(session.Object, create_commit(0), cancellation_token).GetAwaiter().GetResult();

    It should_not_update_any_aggregate_versions = verify_no_more_updates;
}