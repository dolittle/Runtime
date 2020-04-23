// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartitionValidator.when_validating
{
    [Ignore("Not implemented")]
    public class and_both_old_and_new_filter_did_not_include_any_events : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            event_types_fetcher
                .Setup(_ => _.FetchInRange(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Enumerable.Empty<Artifact>()));
            events_fetcher
                .Setup(_ => _.FetchRange(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPositionRange>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Enumerable.Empty<StreamEvent>()));
        };

        Because of = () => result = validator.Validate(filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}