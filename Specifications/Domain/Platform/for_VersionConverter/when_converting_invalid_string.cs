// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Domain.Platform.for_VersionConverter;

public class when_converting_invalid_string : given.a_version_converter
{
    static Exception result;
    Because of = () => result = Catch.Exception((Action) (() => version_converter.FromString("Blah")));

    It should_throw_invalid_version_string = () => result.Should().BeOfType(typeof(InvalidVersionString));
}