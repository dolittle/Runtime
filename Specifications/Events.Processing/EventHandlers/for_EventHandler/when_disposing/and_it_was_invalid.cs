// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler;

public class and_it_was_invalid : given.an_event_handler_with_non_writeable_target_stream
{
    static Exception result;

    Because of = () => result = Catch.Exception(() => event_handler.Dispose());

    It should_not_throw_an_exception = () => result.Should().BeNull();
}