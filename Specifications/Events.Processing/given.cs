// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Store;
using Dolittle.Security;
using Moq;

namespace Dolittle.Runtime.Events.Processing
{
    public static class given
    {
        public static IRemoteProcessorService a_handler_service(IProcessingResult result)
        {
            var handler_service = a_handler_service_mock();
            handler_service.Setup(_ => _.Handle(Moq.It.IsAny<CommittedEventEnvelope>(), Moq.It.IsAny<EventProcessorId>())).Returns(Task.FromResult(result));
            return handler_service.Object;
        }

        public static Mock<IRemoteProcessorService> a_handler_service_mock() => new Moq.Mock<IRemoteProcessorService>();

        public static IEventProcessor an_event_processor(EventProcessorId id, IProcessingResult result)
        {
            var event_processor_mock = an_event_processor_mock();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_processor_mock.Setup(_ => _.Process(Moq.It.IsAny<CommittedEventEnvelope>())).Returns(Task.FromResult(result));
            return event_processor_mock.Object;
        }

        public static IEventProcessor an_event_processor(EventProcessorId id, Func<CommittedEventEnvelope, Task<IProcessingResult>> result_callback)
        {
            var event_processor_mock = an_event_processor_mock();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_processor_mock.Setup(_ => _.Process(Moq.It.IsAny<CommittedEventEnvelope>())).Returns(result_callback);
            return event_processor_mock.Object;
        }

        public static Mock<IFetchNextEvent> a_next_event_fetcher_mock() => new Mock<IFetchNextEvent>();

        public static Mock<IEventProcessor> an_event_processor_mock() => new Mock<IEventProcessor>();

        public static CommittedEventEnvelope a_committed_event_envelope => new CommittedEventEnvelope(
                1,
                new EventMetadata(
                            EventId.New(),
                            new VersionedEventSource(EventSourceId.New(), ArtifactId.New()),
                            Guid.NewGuid(),
                            new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                            DateTimeOffset.UtcNow,
                            new OriginalContext(Application.New(), BoundedContext.New(), Guid.NewGuid(), "", Claims.Empty, null)),
                new PropertyBag(new NullFreeDictionary<string, object>()));
    }
}