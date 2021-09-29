// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams;
using System;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating
{
    public class and_there_is_no_persisted_stream_processor_state : given.all_dependencies
    {
        Establish context = () =>
         {
             stream_processor_state_repository
                 .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
                 .Returns(Task.FromResult(Try<IStreamProcessorState>.Failed(new StreamProcessorStateDoesNotExist(stream_processor_id))));
         };
        static FilterValidationResult result;
        Because of = () => result = filter_validators().Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

        It should_not_fail_validation = () => result.Success.ShouldBeTrue();
    }
}
