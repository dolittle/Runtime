// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class hh_mm_ss : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("hh:mm:ss", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => string.IsNullOrEmpty(error).ShouldBeTrue();
}

public class HH_mm : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("HH:mm", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => string.IsNullOrEmpty(error).ShouldBeTrue();
}
public class HH : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("HH", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => string.IsNullOrEmpty(error).ShouldBeTrue();
}