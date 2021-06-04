// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given
{
    public class an_event_handler_with_non_writeable_target_stream : all_dependencies
    {
         protected static EventHandler event_handler;

        Establish context = () =>
        {
           arguments = new(
                execution_context,
                StreamId.EventLog.Value,
                Array.Empty<ArtifactId>(),
                false,
                scope);

            event_handler = new EventHandler(
                stream_processors.Object,
                filter_validation.Object,
                stream_definitions.Object,
                reverse_call_dispatcher.Object,
                arguments,
                factory_for_stream_writer,
                logger_factory.Object,
                cancellation_token
            );
        };
    }
}