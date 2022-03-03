// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Versioning.for_VersionConverter;

public class when_converting_with_only_major_minor_patch : given.a_version_converter
{
    static Version version;

    Because of = () => version = version_converter.FromString("1.2.3");

    It should_hold_correct_major = () => version.Major.ShouldEqual(1);
    It should_hold_correct_minor = () => version.Minor.ShouldEqual(2);
    It should_hold_correct_patch = () => version.Patch.ShouldEqual(3);
    It should_hold_correct_build = () => version.Build.ShouldEqual(0);
    It should_be_considered_as_release_build = () => version.IsPreRelease.ShouldBeFalse();
    It should_not_hold_a_pre_release_string = () => version.PreReleaseString.ShouldBeEmpty();
}