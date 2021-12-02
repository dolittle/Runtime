// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_NullFreeDictionary;

public class when_creating_dictionary_from_collection_initializer
{
    static NullFreeDictionary<string, string> null_free_dictionary;

    Because of = () => null_free_dictionary = new NullFreeDictionary<string, string>()
    {
        {
            "k1", "e1"
        },
        {
            "k2", null
        },
        {
            "k3", "e3"
        }
    };

    It should_not_be_null = () => null_free_dictionary.ShouldNotBeNull();
    It should_not_be_empty = () => null_free_dictionary.ShouldNotBeEmpty();
    It should_have_first_key = () => null_free_dictionary.ContainsKey("k1").ShouldBeTrue();
    It should_have_first_element = () => null_free_dictionary["k1"].ShouldEqual("e1");
    It should_have_first_key_value_pair = () => null_free_dictionary.Contains(new System.Collections.Generic.KeyValuePair<string, string>("k1", "e1")).ShouldBeTrue();
    It should_not_have_second_key = () => null_free_dictionary.ContainsKey("K2").ShouldBeFalse();
    It should_have_third_key = () => null_free_dictionary.ContainsKey("k3").ShouldBeTrue();
    It should_have_third_element = () => null_free_dictionary["k3"].ShouldEqual("e3");
    It should_have_third_key_value_pair = () => null_free_dictionary.Contains(new System.Collections.Generic.KeyValuePair<string, string>("k3", "e3")).ShouldBeTrue();
}