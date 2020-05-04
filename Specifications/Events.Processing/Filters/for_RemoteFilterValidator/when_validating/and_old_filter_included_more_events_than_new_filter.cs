// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_RemoteFilterValidator.when_validating
{
    [Ignore("Not implemented")]
    public class and_old_filter_included_more_events_than_new_filter : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            event_types_fetcher
                .Setup(_ => _.FetchInRange(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Artifact[] { given.artifacts.single() }.AsEnumerable()));
            events_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new StreamEvent[] { given.stream_event.single() }.AsEnumerable()));
            filter_processor
                .Setup(_ => _.Filter(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<EventProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IFilterResult>(new FailedFiltering("")));
        };

        Because of = () => result = validator.Validate(filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();

        It should_fail_validation = () => result.Succeeded.ShouldBeFalse();
    }
}