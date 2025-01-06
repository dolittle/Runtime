// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Processing.Filters.for_AbstractFilterProcessor.when_processing;

public class and_event_is_not_included : given.all_dependencies
{
    static IProcessingResult result;
    static PartitionId partition;

    Establish context = () =>
    {
        partition = "   weird partition   ";
        filter_processor
            .Setup(_ => _.Filter(
                Moq.It.IsAny<CommittedEvent>(),
                Moq.It.IsAny<PartitionId>(),
                Moq.It.IsAny<EventProcessorId>(),
                Moq.It.IsAny<ExecutionContext>(),
                Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IFilterResult>(new SuccessfulFiltering(false, partition)));
    };

    Because of = () => result = filter_processor.Object.Process(committed_event, partition, StreamPosition.Start, committed_event.ExecutionContext, CancellationToken.None).GetAwaiter().GetResult();
    It should_not_write_stream = () => events_to_streams_writer.Verify(_ => _.Write(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Times.Never);
    It should_return_successful_processing_result = () => result.Succeeded.ShouldBeTrue();
}