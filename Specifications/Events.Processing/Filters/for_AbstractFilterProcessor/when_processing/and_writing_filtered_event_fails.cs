// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Processing.Filters.for_AbstractFilterProcessor.when_processing;

public class and_writing_filtered_event_fails : given.all_dependencies
{
    static IProcessingResult result;
    static PartitionId partition;

    Establish context = () =>
    {
        events_to_streams_writer
            .Setup(_ => _.Write(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("some error"));
        partition = "   weird partition   ";
        filter_processor
            .Setup(_ => _.Filter(
                Moq.It.IsAny<CommittedEvent>(),
                Moq.It.IsAny<PartitionId>(),
                Moq.It.IsAny<EventProcessorId>(),
                Moq.It.IsAny<ExecutionContext>(),
                Moq.It.IsAny<CancellationToken>())).ReturnsAsync(new SuccessfulFiltering(true));
    };

    Because of = () => result = filter_processor.Object.Process(committed_event, partition, StreamPosition.Start, committed_event.ExecutionContext, CancellationToken.None).GetAwaiter().GetResult();
    It should_write_stream = () => events_to_streams_writer.Verify(_ => _.Write(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Times.Once);
    It should_return_failed_result = () => result.Succeeded.ShouldBeFalse();
}