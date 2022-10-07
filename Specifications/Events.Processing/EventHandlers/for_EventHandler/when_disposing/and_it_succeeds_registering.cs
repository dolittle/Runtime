// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler;

[Ignore("Need to mock StreamProcessor")]
public class and_it_succeeds_registering : given.an_event_handler
{
    Establish context = () =>
    {
        stream_processors
            .Setup(_ => _
                .TryCreateAndRegister(
                    event_handler.Scope,
                    event_handler.EventProcessor,
                    IsAny<EventProcessorKind>(),
                    IsAny<EventLogStreamDefinition>(),
                    IsAny<Func<TenantId, IEventProcessor>>(),
                    IsAny<ExecutionContext>(),
                    IsAny<CancellationToken>()))
            .Returns(Try<StreamProcessor>.Succeeded(null));

        stream_processors
            .Setup(_ => _
                .TryCreateAndRegister(
                    event_handler.Scope,
                    event_handler.EventProcessor,
                    IsAny<EventProcessorKind>(),
                    event_handler.FilteredStreamDefinition,
                    IsAny<Func<TenantId, IEventProcessor>>(),
                    IsAny<ExecutionContext>(),
                    IsAny<CancellationToken>()))
            .Returns(Try<StreamProcessor>.Succeeded(null));
    };

    Because of = () => event_handler.RegisterAndStart().GetAwaiter().GetResult();

    It should_try_to_register_filter_processor = () => stream_processors.Verify(_ => _
        .TryCreateAndRegister(
            event_handler.Scope,
            event_handler.EventProcessor,
            IsAny<EventProcessorKind>(),
            IsAny<EventLogStreamDefinition>(),
            IsAny<Func<TenantId, IEventProcessor>>(),
            IsAny<ExecutionContext>(),
            IsAny<CancellationToken>()), Once());

    It should_accept_event_handler = () => reverse_call_dispatcher.Verify(_ => _.Accept(IsAny<EventHandlerRegistrationResponse>(), IsAny<CancellationToken>()), Once);
    It should_not_reject = () => reverse_call_dispatcher.Verify(_ => _.Reject(IsAny<EventHandlerRegistrationResponse>(), IsAny<CancellationToken>()), Never);
}
