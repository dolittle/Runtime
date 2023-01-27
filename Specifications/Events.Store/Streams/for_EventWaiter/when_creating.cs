// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_EventWaiter;

public class when_creating
{
    static ScopeId scope;
    static StreamId stream;
    static EventWaiterId expected_id;
    static EventWaiter result;

    Establish context = () =>
    {
        scope = new ScopeId(Guid.Parse("9e0370f7-d1ed-4a2c-94af-45243fab0be6"));
        stream = Guid.Parse("07690cac-45cd-4aa4-bd45-de28f8e27661");
        expected_id = new EventWaiterId(scope, stream);
    };

    Because of = () => result = new EventWaiter(scope, stream);

    It should_have_the_correct_id = () => result.Id.Should().Be(expected_id);
}