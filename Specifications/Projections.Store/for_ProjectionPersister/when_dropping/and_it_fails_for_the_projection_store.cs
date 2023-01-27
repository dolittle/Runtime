// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_dropping;

public class and_it_fails_for_the_projection_store : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        projection_store
            .Setup(_ => _.TryDrop(projection_one_id, projection_one_scope, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result;
    Because of = () => result = projection_persister.TryDrop(projection_one, cancellation_token).GetAwaiter().GetResult();

    It should_return_a_failed_result = () => result.Should().BeFalse();
    It should_have_dropped_it_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Once);
    It should_have_dropped_it_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Once);
    It should_have_dropped_it_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Once);
    It should_have_dropped_it_from_the_projection_store = () => projection_store.Verify(_ => _.TryDrop(projection_one_id, projection_one_scope, cancellation_token), Times.Once);
    It should_increment_total_drop_attempts_once = () => metrics.Verify(_ => _.IncrementTotalDropAttempts(), Times.Once);
    It should_increment_total_copy_store_drops_three_times = () => metrics.Verify(_ => _.IncrementTotalCopyStoreDrops(), Times.Exactly(3));
}