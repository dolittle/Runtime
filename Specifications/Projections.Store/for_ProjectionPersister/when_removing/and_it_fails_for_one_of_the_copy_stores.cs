// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_removing;

public class and_it_fails_for_one_of_the_copy_stores : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_one
            .Setup(_ => _.TryRemove(projection_two, key_one, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result;
    Because of = () => result = projection_persister.TryRemove(projection_two, key_one, cancellation_token).GetAwaiter().GetResult();

    It should_return_a_failed_result = () => result.ShouldBeFalse();
    It should_have_removed_it_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryRemove(projection_two, key_one, cancellation_token), Times.Once);
    It should_not_have_removed_it_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryRemove(projection_two, key_one, cancellation_token), Times.Never);
    It should_not_have_removed_it_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryRemove(projection_two, key_one, cancellation_token), Times.Never);
    It should_not_have_removed_it_from_the_projection_store = () => projection_store.Verify(_ => _.TryRemove(projection_two_id, projection_two_scope, key_one, cancellation_token), Times.Never);
}