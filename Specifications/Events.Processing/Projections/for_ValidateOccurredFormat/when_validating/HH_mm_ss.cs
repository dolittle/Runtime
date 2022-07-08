// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class HH_mm_ss : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("HH:mm:ss", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => error.ShouldBeNull();
}