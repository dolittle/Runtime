// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_dropping;

public class and_only_one_copy_store_should_persist : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(projection_one))
            .Returns(false);

        copy_store_three
            .Setup(_ => _.ShouldPersistFor(projection_one))
            .Returns(false);
    };

    static bool result;
    Because of = () => result = projection_persister.TryDrop(projection_one, cancellation_token).GetAwaiter().GetResult();

    It should_return_a_successful_result = () => result.ShouldBeTrue();
    It should_not_have_dropped_it_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Never);
    It should_have_dropped_it_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Once);
    It should_not_have_dropped_it_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Never);
    It should_have_dropped_it_from_the_projection_store = () => projection_store.Verify(_ => _.TryDrop(projection_one_id, projection_one_scope, cancellation_token), Times.Once);
    It should_increment_total_drop_attempts_once = () => metrics.Verify(_ => _.IncrementTotalDropAttempts(), Times.Once);
    It should_increment_total_copy_store_drops_once = () => metrics.Verify(_ => _.IncrementTotalCopyStoreDrops(), Times.Once);
}