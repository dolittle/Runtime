// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Domain.Platform.for_Version;

public class when_instantiating_with_pre_release_tag
{
    static Version result;
    Because of = () => result = new Version(1, 2, 3, 4, "alpha2");
    It should_be_considered_a_pre_release = () => result.IsPreRelease.Should().BeTrue();
}