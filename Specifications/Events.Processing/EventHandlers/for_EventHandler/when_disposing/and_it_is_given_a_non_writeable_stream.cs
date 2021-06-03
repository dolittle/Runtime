// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler
{

    public class and_it_is_given_a_non_writeable_stream : given.an_event_handler_with_non_writeable_target_stream
    {
        Because of = async () => await event_handler.RegisterAndStart().ConfigureAwait(false);

        It should_reject_with_cannot_register_event_handler_on_writeable_stream = () => failure.Id.ShouldEqual(EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream);
    }
}