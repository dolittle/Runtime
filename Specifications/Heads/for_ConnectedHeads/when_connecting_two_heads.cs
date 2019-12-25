// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Heads.for_ConnectedHeads
{
    public class when_connecting_two_heads : given.two_heads
    {
        static ConnectedHeads connected_heads;

        Establish context = () => connected_heads = new ConnectedHeads(Moq.Mock.Of<ILogger>());

        Because of = () =>
        {
            connected_heads.Connect(first_head);
            connected_heads.Connect(second_head);
        };

        It should_have_both_heads_as_connected_heads = () => connected_heads.GetAll().ShouldContainOnly(new[] { first_head, second_head });
    }
}