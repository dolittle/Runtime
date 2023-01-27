// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Autofac;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.when_validating;

public class and_getting_the_stream_processor_state_fails : given.all_dependencies
{
    Establish context = () =>
    {
        stream_processor_state_repository
            .Setup(_ => _.TryGetFor(stream_processor_id, cancellation_token))
            .Returns(Task.FromResult(Try<IStreamProcessorState>.Failed(new Exception())));
    };

    static FilterValidationResult result;
    Because of = () => result = filter_validators_with_services(_ => _.RegisterInstance(filter_validator.Object)).Validate(filter_processor, cancellation_token).GetAwaiter().GetResult();

    It should_fail_validation = () => result.Success.Should().BeFalse();
}