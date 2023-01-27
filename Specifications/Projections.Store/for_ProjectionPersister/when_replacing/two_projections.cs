// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_replacing;

public class two_projections : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(projection_one))
            .Returns(false);
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);

        copy_store_one
            .Setup(_ => _.TryReplace(projection_one, key_two, state_two, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result_one;
    static bool result_two;
    Because of = () =>
    {
        result_one = projection_persister.TryReplace(projection_one, key_two, state_two, cancellation_token).GetAwaiter().GetResult();
        result_two = projection_persister.TryReplace(projection_two, key_one, state_one, cancellation_token).GetAwaiter().GetResult();
    };

    It should_return_a_successful_result_for_projection_one = () => result_one.Should().BeTrue();
    It should_not_have_replaced_projection_one_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryReplace(projection_one, key_two, state_two, cancellation_token), Times.Never);
    It should_have_replaced_projection_one_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryReplace(projection_one, key_two, state_two, cancellation_token), Times.Once);
    It should_have_replaced_projection_one_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryReplace(projection_one, key_two, state_two, cancellation_token), Times.Once);
    It should_have_replaced_projection_one_from_the_projection_store = () => projection_store.Verify(_ => _.TryReplace(projection_one_id, projection_one_scope, key_two, state_two, cancellation_token), Times.Once);

    It should_return_a_successful_result_for_projection_two = () => result_two.Should().BeTrue();
    It should_not_have_replaced_projection_two_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Never);
    It should_have_replaced_projection_two_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Once);
    It should_have_replaced_projection_two_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Once);
    It should_have_replaced_projection_two_from_the_projection_store = () => projection_store.Verify(_ => _.TryReplace(projection_two_id, projection_two_scope, key_one, state_one, cancellation_token), Times.Once);
    
    It should_increment_total_replace_attempts_twice = () => metrics.Verify(_ => _.IncrementTotalReplaceAttempts(), Times.Exactly(2));
    It should_increment_total_copy_store_replacements_four_times = () => metrics.Verify(_ => _.IncrementTotalCopyStoreReplacements(), Times.Exactly(4));
}