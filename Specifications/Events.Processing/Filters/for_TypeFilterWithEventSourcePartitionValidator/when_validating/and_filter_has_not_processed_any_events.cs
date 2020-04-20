// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_TypeFilterWithEventSourcePartitionValidator.when_validating
{
    public class and_filter_has_not_processed_any_events : given.all_dependencies
    {
        static Exception exception;
        Establish context = () => streams_metadata.Setup(_ => _.GetLastProcessedEventLogSequenceNumber(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult<EventLogSequenceNumber>(null));

        Because of = () => exception = Catch.Exception(() => validator.Validate(filter_processor.Object).GetAwaiter().GetResult());

        It should_not_fail_validation = () => exception.ShouldBeNull();
    }
}