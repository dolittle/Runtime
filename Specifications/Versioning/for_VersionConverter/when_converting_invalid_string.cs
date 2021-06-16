// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Versioning.for_VersionConverter
{
    public class when_converting_invalid_string : given.a_version_converter
    {
        static Exception result;
        Because of = () => result = Catch.Exception(() => version_converter.FromString("Blah"));

        It should_throw_invalid_version_string = () => result.ShouldBeOfExactType(typeof(InvalidVersionString));
    }
}