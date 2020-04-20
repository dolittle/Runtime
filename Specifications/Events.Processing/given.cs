// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Moq;

namespace Dolittle.Runtime.Events.Processing
{
    public static class given
    {
        public static Mock<IEventProcessor> an_event_processor() => new Mock<IEventProcessor>();

        public static Mock<IEventProcessor> an_event_processor(EventProcessorId id, IProcessingResult result) => an_event_processor(id, (result, Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CommittedEvent>()));

        public static Mock<IEventProcessor> an_event_processor(EventProcessorId id, params (IProcessingResult result, PartitionId partition_Id, CommittedEvent @event)[] event_and_result_pairs)
        {
            var event_processor_mock = an_event_processor();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_and_result_pairs.ForEach(pair => event_processor_mock.Setup(_ => _.Process(pair.@event, pair.partition_Id, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(pair.result)));
            return event_processor_mock;
        }

        public static Mock<IEventProcessor> an_event_processor(EventProcessorId id, Func<CommittedEvent, PartitionId, CancellationToken, Task<IProcessingResult>> callback)
        {
            var event_processor_mock = an_event_processor();
            event_processor_mock.SetupGet(_ => _.Identifier).Returns(id);
            event_processor_mock.Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>())).Returns(callback);
            return event_processor_mock;
        }

        public static Mock<IFetchEventsFromStreams> a_next_event_fetcher() => new Mock<IFetchEventsFromStreams>();
    }
}