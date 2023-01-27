// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_removing;

public class two_projections : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_two
            .Setup(_ => _.ShouldPersistFor(projection_one))
            .Returns(false);
        copy_store_three
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);

        copy_store_two
            .Setup(_ => _.TryRemove(projection_two, key_two, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result_one;
    static bool result_two;
    Because of = () =>
    {
        result_one = projection_persister.TryRemove(projection_one, key_one, cancellation_token).GetAwaiter().GetResult();
        result_two = projection_persister.TryRemove(projection_two, key_two, cancellation_token).GetAwaiter().GetResult();
    };

    It should_return_a_successful_result_for_projection_one = () => result_one.Should().BeTrue();
    It should_have_removed_projection_one_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryRemove(projection_one, key_one, cancellation_token), Times.Once);
    It should_not_have_removed_projection_one_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryRemove(projection_one, key_one, cancellation_token), Times.Never);
    It should_have_removed_projection_one_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryRemove(projection_one, key_one, cancellation_token), Times.Once);
    It should_have_removed_projection_one_from_the_projection_store = () => projection_store.Verify(_ => _.TryRemove(projection_one_id, projection_one_scope, key_one, cancellation_token), Times.Once);

    It should_return_a_failed_result_for_projection_two = () => result_two.Should().BeFalse();
    It should_have_removed_projection_two_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryRemove(projection_two, key_two, cancellation_token), Times.Once);
    It should_have_removed_projection_two_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryRemove(projection_two, key_two, cancellation_token), Times.Once);
    It should_not_have_removed_projection_two_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryRemove(projection_two, key_two, cancellation_token), Times.Never);
    It should_not_have_removed_projection_two_from_the_projection_store = () => projection_store.Verify(_ => _.TryRemove(projection_two_id, projection_two_scope, key_two, cancellation_token), Times.Never);
    
    It should_increment_total_remove_attempts_twice = () => metrics.Verify(_ => _.IncrementTotalRemoveAttempts(), Times.Exactly(2));
    It should_increment_total_copy_store_removals_three_times = () => metrics.Verify(_ => _.IncrementTotalCopyStoreRemovals(), Times.Exactly(3));
    It should_increment_total_copy_store_failures_once = () => metrics.Verify(_ => _.IncrementTotalFailedCopyStoreRemovals(), Times.Once);
}