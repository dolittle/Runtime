// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Processing.Filters.for_AbstractFilterProcessor.when_processing
{
    public class and_event_is_not_included : given.all_dependencies
    {
        static IFilterResult result;

        Establish context = () => filter_processor.Setup(_ => _.Filter(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<EventProcessorId>(),
            Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IFilterResult>(new SucceededFilteringResult(false, PartitionId.NotSet)));

        Because of = () => result = filter_processor.Object.Process(committed_event, PartitionId.NotSet).Result as IFilterResult;

        It should_not_write_it_to_any_stream = () => events_to_streams_writer.Verify(_ => _.Write(committed_event, Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Times.Never);
    }
}