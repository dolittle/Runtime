// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.for_DictionaryTypeExtensions
{
    public class when_checking_if_regular_dictionary_is_readonly
    {
        static bool result;

        Because of = () => result = typeof(Dictionary<string, string>).IsReadOnlyDictionary();

        It should_not_be_considered_a_readonly_dictionary = () => result.ShouldBeFalse();
    }
}