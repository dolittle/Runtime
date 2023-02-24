// // Copyright (c) Dolittle. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// using System;
// using System.Threading;
// using Dolittle.Runtime.Domain.Tenancy;
// using Dolittle.Runtime.Events.Processing.Contracts;
// using Dolittle.Runtime.Events.Store;
// using Dolittle.Runtime.Events.Store.Streams;
// using Machine.Specifications;
// using static Moq.It;
// using static Moq.Times;
// using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
//
// namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler;
//
// public class and_it_is_given_a_non_writeable_stream : given.an_event_handler_with_non_writeable_target_stream
// {
//     Because of = () => event_handler.RegisterAndStart().GetAwaiter().GetResult();
//
//
//     It should_reject = () => reverse_call_dispatcher.Verify(_ => _.Reject(IsAny<EventHandlerRegistrationResponse>(), IsAny<CancellationToken>()), Once);
//     It should_not_accept_event_handler = () => reverse_call_dispatcher.Verify(_ => _.Accept(IsAny<EventHandlerRegistrationResponse>(), IsAny<CancellationToken>()), Never);
//     It should_reject_with_cannot_register_event_handler_on_writeable_stream = () => failure.Id.ShouldEqual(EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream);
//     It should_not_register_any_stream_processors = () => stream_processors.Verify(_ => _
//         .TryCreateAndRegister(
//             IsAny<ScopeId>(),
//             IsAny<EventProcessorId>(),
//             IsAny<EventProcessorKind>(),
//             IsAny<IStreamDefinition>(),
//             IsAny<Func<TenantId, IEventProcessor>>(),
//             IsAny<ExecutionContext>(),
//             IsAny<CancellationToken>()), Moq.Times.Never());
// }
