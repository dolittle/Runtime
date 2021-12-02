// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.for_DictionaryTypeExtensions;

public class when_checking_type_deriving_from_readonly_dictionary_if_is_readonly
{
    class derived : ReadOnlyDictionary<string, string>
    {
        derived()
            : base(null)
        {
        }
    }

    static bool result;

    Because of = () => result = typeof(derived).IsReadOnlyDictionary();

    It should_be_considered_a_readonly_dictionary = () => result.ShouldBeTrue();
}