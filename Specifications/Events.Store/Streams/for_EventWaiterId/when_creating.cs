// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiterId
{
    public class when_creating
    {
        static ScopeId scope;
        static StreamId stream;
        static EventWaiterId result;

        Establish context = () =>
        {
            scope = new ScopeId(Guid.Parse("9e0370f7-d1ed-4a2c-94af-45243fab0be6"));
            stream = Guid.Parse("07690cac-45cd-4aa4-bd45-de28f8e27661");
        };

        Because of = () => result = new EventWaiterId(scope, stream);

        It should_have_the_correct_scope = () => result.Scope.ShouldEqual(scope);
        It should_have_the_correct_stream = () => result.Stream.ShouldEqual(stream);
    }
}