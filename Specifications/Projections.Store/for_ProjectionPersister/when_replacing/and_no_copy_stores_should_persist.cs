// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_replacing;

public class and_no_copy_stores_should_persist : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);
        copy_store_two
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);
        copy_store_three
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);
    };

    static bool result;
    Because of = () => result = projection_persister.TryReplace(projection_two, key_one, state_one, cancellation_token).GetAwaiter().GetResult();

    It should_return_a_successful_result = () => result.ShouldBeTrue();
    It should_not_have_replaced_it_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Never);
    It should_not_have_replaced_it_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Never);
    It should_not_have_replaced_it_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryReplace(projection_two, key_one, state_one, cancellation_token), Times.Never);
    It should_have_replaced_it_from_the_projection_store = () => projection_store.Verify(_ => _.TryReplace(projection_two_id, projection_two_scope, key_one, state_one, cancellation_token), Times.Once);
}