// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_definition_has_changed_but_filter_has_not_processed_any_events : given.all_dependencies
{

    Establish context = () =>
    {
        stream_processor_state_repository
            .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
            .Returns(Task.FromResult(Try<IStreamProcessorState>.Succeeded(StreamProcessorState.New)));

        filter_definitions
            .Setup(_ => _.TryGetFromStream(scope_id, filter_target_stream, cancellation_token))
            .Returns(Task.FromResult(Try<IFilterDefinition>.Succeeded(different_filter_definition)));

        filter_validator
            .Setup(_ => _.Validate(filter_definition, filter_processor, StreamPosition.Start, cancellation_token))
            .Returns(Task.FromResult(FilterValidationResult.Failed("something went wrong")));
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators().Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

    It should_not_fail_validation = () => result.Success.ShouldBeTrue();
}