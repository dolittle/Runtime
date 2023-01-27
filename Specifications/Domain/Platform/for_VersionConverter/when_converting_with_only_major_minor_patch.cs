// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Domain.Platform.for_VersionConverter;

public class when_converting_with_only_major_minor_patch : given.a_version_converter
{
    static Version version;

    Because of = () => version = version_converter.FromString("1.2.3");

    It should_hold_correct_major = () => version.Major.Should().Be(1);
    It should_hold_correct_minor = () => version.Minor.Should().Be(2);
    It should_hold_correct_patch = () => version.Patch.Should().Be(3);
    It should_hold_correct_build = () => version.Build.Should().Be(0);
    It should_be_considered_as_release_build = () => version.IsPreRelease.Should().BeFalse();
    It should_not_hold_a_pre_release_string = () => version.PreReleaseString.Should().BeEmpty();
}