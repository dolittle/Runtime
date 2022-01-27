// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.for_ProjectionPersister.when_dropping;

public class two_projections : given.a_persister_with_three_copy_stores_and_two_projections
{
    Establish context = () =>
    {
        copy_store_one
            .Setup(_ => _.ShouldPersistFor(projection_one))
            .Returns(false);
        copy_store_two
            .Setup(_ => _.ShouldPersistFor(projection_two))
            .Returns(false);

        copy_store_two
            .Setup(_ => _.TryDrop(projection_one, cancellation_token))
            .ReturnsAsync(false);
    };

    static bool result_one;
    static bool result_two;
    Because of = () =>
    {
        result_one = projection_persister.TryDrop(projection_one, cancellation_token).GetAwaiter().GetResult();
        result_two = projection_persister.TryDrop(projection_two, cancellation_token).GetAwaiter().GetResult();
    };

    It should_return_a_failed_result_for_projection_one = () => result_one.ShouldBeFalse();
    It should_not_have_dropped_projection_one_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Never);
    It should_have_dropped_projection_one_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Once);
    It should_not_have_dropped_projection_one_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryDrop(projection_one, cancellation_token), Times.Never);
    It should_not_have_dropped_projection_one_from_the_projection_store = () => projection_store.Verify(_ => _.TryDrop(projection_one_id, projection_one_scope, cancellation_token), Times.Never);

    It should_return_a_successful_result_for_projection_two = () => result_two.ShouldBeTrue();
    It should_have_dropped_projection_two_from_the_first_copy_store = () => copy_store_one.Verify(_ => _.TryDrop(projection_two, cancellation_token), Times.Once);
    It should_not_have_dropped_projection_two_from_the_second_copy_store = () => copy_store_two.Verify(_ => _.TryDrop(projection_two, cancellation_token), Times.Never);
    It should_have_dropped_projection_two_from_the_third_copy_store = () => copy_store_three.Verify(_ => _.TryDrop(projection_two, cancellation_token), Times.Once);
    It should_have_dropped_projection_two_from_the_projection_store = () => projection_store.Verify(_ => _.TryDrop(projection_two_id, projection_two_scope, cancellation_token), Times.Once);
}