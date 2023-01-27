// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class null_string : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid(null, out error);

    It should_not_be_valid = () => is_valid.Should().BeFalse();
    It should_output_error = () => error.Should().NotBeNull();
}