// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.for_ObjectToDictionaryExtensions;

public class when_converting_object_with_multiple_properties
{
    static ObjectWithMultipleProperties the_object;
    static IDictionary<string, object> result;

    Establish context = () => the_object = new ObjectWithMultipleProperties
    {
        TheString = "Fourty Two",
        TheInt = 42,
        TheDouble = 42.42
    };

    Because of = () => result = the_object.ToDictionary();

    It should_hold_the_string = () => result["TheString"].ShouldEqual(the_object.TheString);
    It should_hold_the_int = () => result["TheInt"].ShouldEqual(the_object.TheInt);
    It should_hold_the_double = () => result["TheDouble"].ShouldEqual(the_object.TheDouble);
}