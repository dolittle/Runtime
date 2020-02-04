// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Events;
using Dolittle.Runtime.Events.Store;
using Dolittle.Security;
using Moq;

namespace Dolittle.Runtime.Events.Processing
{
    public static class given
    {
        class MyEvent : IEvent
        {
        }

        public static IRemoteProcessorService a_remote_processor_service(IProcessingResult result)
        {
            var handler_service = a_remote_processor_mock();
            handler_service.Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<EventProcessorId>())).Returns(Task.FromResult(result));
            return handler_service.Object;
        }

        public static Mock<IRemoteProcessorService> a_remote_processor_mock() => new Moq.Mock<IRemoteProcessorService>();

        public static IRemoteFilterService a_remote_filter_service(IFilterResult result)
        {
            var handler_service = a_remote_filter_mock();
            handler_service.Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<EventProcessorId>())).Returns(Task.FromResult(result));
            return handler_service.Object;
        }

        public static Mock<IRemoteFilterService> a_remote_filter_mock() => new Moq.Mock<IRemoteFilterService>();

        public static Mock<IEventProcessor> an_event_processor_mock(EventProcessorId id, IProcessingResult result) => an_event_processor_mock(id, (result, Moq.It.IsAny<CommittedEvent>()));

        public static Mock<IEventProcessor> an_event_processor_mock(EventProcessorId id, params (IProcessingResult result, CommittedEvent @event)[] event_and_result_pairs)
        {
            var event_processor_mock = an_event_processor_mock();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_and_result_pairs.ForEach(pair => event_processor_mock.Setup(_ => _.Process(pair.@event)).Returns(Task.FromResult(pair.result)));
            return event_processor_mock;
        }

        public static Mock<IEventProcessor> an_event_processor_mock(EventProcessorId id, Func<CommittedEvent, Task<IProcessingResult>> callback)
        {
            var event_processor_mock = an_event_processor_mock();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_processor_mock.Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>())).Returns(callback);
            return event_processor_mock;
        }

        public static Mock<IFetchNextEvent> a_next_event_fetcher_mock() => new Mock<IFetchNextEvent>();

        public static Mock<IEventProcessor> an_event_processor_mock() => new Mock<IEventProcessor>();

        public static CommittedEvent a_committed_event => new CommittedEvent(
                new CommittedEventVersion(1, 1, 1),
                new EventMetadata(
                    EventId.New(),
                    new VersionedEventSource(EventSourceId.New(), ArtifactId.New()),
                    Guid.NewGuid(),
                    new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                    DateTimeOffset.UtcNow,
                    new OriginalContext(Application.New(), BoundedContext.New(), Guid.NewGuid(), "", Claims.Empty, null)),
                new MyEvent());
    }
}