// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Filters.for_ValidateFilterByComparingStreams.when_validating;

public class and_filter_has_not_processed_any_events : given.all_dependencies
{
    static FilterValidationResult result;

    Because of = () => result = validator.Validate(filter_definition, filter_processor.Object, 0, CancellationToken.None).GetAwaiter().GetResult();
    It should_not_fail_validation = () => result.Success.ShouldBeTrue();
}