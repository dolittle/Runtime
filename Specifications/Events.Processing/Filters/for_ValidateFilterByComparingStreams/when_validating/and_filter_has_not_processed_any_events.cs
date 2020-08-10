// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating
{
    public class and_filter_has_not_processed_any_events : given.all_dependencies
    {
        static FilterValidationResult result;

        Establish context = () =>
        {
            stream_processor_states
                .Setup(_ => _.TryGetFor(Moq.It.IsAny<IStreamProcessorId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((true, StreamProcessorState.New)));
        };

        Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, CancellationToken.None).GetAwaiter().GetResult();
        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
    }
}