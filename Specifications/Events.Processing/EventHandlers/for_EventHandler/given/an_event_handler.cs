// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given;

public class an_event_handler : all_dependencies
{
    protected static EventHandler event_handler;

    Establish context = () =>
    {
        event_handler = new EventHandler(
            stream_processors.Object,
            filter_validation.Object,
            stream_definitions.Object,
            reverse_call_dispatcher.Object,
            arguments,
            factory_for_stream_writer,
            logger_factory,
            execution_context,
            cancellation_token
        );
    };
}