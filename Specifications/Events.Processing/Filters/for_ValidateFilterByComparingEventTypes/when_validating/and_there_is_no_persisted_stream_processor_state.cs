// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;
using System;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingEventTypes.when_validating
{
    public class and_there_is_no_persisted_stream_processor_state : given.all_dependencies
    {
        Establish context = () =>
        {
            stream_processor_states
                .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                .Returns(Task.FromResult<Try<IStreamProcessorState>>((false, null)));
        };

        static FilterValidationResult result;
        Because of = () => result = validator.Validate(filter_definition, filter_processor, cancellation_token).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Succeeded.ShouldBeTrue();
        It should_not_fetch_event_types = () => events_fetchers.VerifyNoOtherCalls();
    }
}