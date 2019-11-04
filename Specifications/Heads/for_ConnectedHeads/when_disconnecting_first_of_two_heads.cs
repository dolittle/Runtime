/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Heads.for_ConnectedHeads
{
    public class when_disconnecting_first_of_two_heads : given.two_heads
    {
        static ConnectedHeads connected_heads;
        Establish context = () =>
        {
            connected_heads = new ConnectedHeads(Moq.Mock.Of<ILogger>());
            connected_heads.Connect(first_head);
            connected_heads.Connect(second_head);
        };

        Because of = () => connected_heads.Disconnect(first_head.HeadId);

        It should_consider_head_disconnected = () => connected_heads.IsConnected(first_head.HeadId);

        It should_only_have_second_head_as_connected_heads = () => connected_heads.GetAll().ShouldContainOnly(new[] { second_head });
    }
}