// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_replacing;

public class and_it_fails_for_one_of_the_copy_stores : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_three
            .Setup(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result;
    Because of = () => result = projection_persister.TryReplace(projection_two, key_one, state_one, cancellation_token).GetAwaiter().GetResult();

    It should_return_a_failed_result = () => result.ShouldBeFalse();
    It should_have_replaced_it_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Once);
    It should_have_replaced_it_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Once);
    It should_have_replaced_it_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Once);
    It should_not_have_replaced_it_from_the_projection_store = () => projection_store.Verify(_ => _.TryReplace(projection_two_id, projection_two_scope, key_one, state_one, cancellation_token), Times.Never);
    It should_increment_total_replace_attempts_once = () => metrics.Verify(_ => _.IncrementTotalReplaceAttempts(), Times.Once);
    It should_increment_total_copy_store_replacements_twice = () => metrics.Verify(_ => _.IncrementTotalCopyStoreReplacements(), Times.Exactly(2));
    It should_increment_total_copy_store_failures_once = () => metrics.Verify(_ => _.IncrementTotalFailedCopyStoreReplacements(), Times.Once);
}