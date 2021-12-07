// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.for_DictionaryTypeExtensions;

public class when_checking_if_dictionary_is_dictionary
{
    static bool result;

    Because of = () => result = typeof(Dictionary<string, string>).IsDictionary();

    It should_be_considered_a_dictionary = () => result.ShouldBeTrue();
}