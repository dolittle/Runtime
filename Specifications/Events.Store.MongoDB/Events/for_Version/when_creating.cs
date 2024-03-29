// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_Version;

public class when_creating
{
    static int major;
    static int minor;
    static int patch;
    static int build;
    static string pre_release;
    static Version result;

    Establish context = () =>
    {
        major = 1943339427;
        minor = 811944027;
        patch = 40905905;
        build = 1275155639;
        pre_release = "Something";
    };

    Because of = () => result = new Version(
        major,
        minor,
        patch,
        build,
        pre_release);

    It should_have_the_correct_major_version_number = () => result.Major.ShouldEqual(major);
    It should_have_the_correct_minor_version_number = () => result.Minor.ShouldEqual(minor);
    It should_have_the_correct_patch_version_number = () => result.Patch.ShouldEqual(patch);
    It should_have_the_correct_build_number = () => result.Build.ShouldEqual(build);
    It should_have_the_correct_pre_release = () => result.PreRelease.ShouldEqual(pre_release);
}