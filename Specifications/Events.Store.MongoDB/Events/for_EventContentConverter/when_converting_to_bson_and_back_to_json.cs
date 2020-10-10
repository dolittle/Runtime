// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventContentConverter
{
    public class when_converting_to_bson_and_back_to_json
    {
        static EventContentConverter content_converter;

        static string original_json;
        static string recovered_json;

        Establish context = () =>
        {
            content_converter = new EventContentConverter();

            var document = JsonDocument.Parse(
                @"{
                    ""null"": null,

                    ""true"": true,
                    ""false"": false,

                    ""empty string"": """",
                    ""short string"": ""some stringy things"",

                    ""positive integer"": 123,
                    ""negative integer"": -321,
                    ""big positive number"": 9223372036854775807,

                    ""empty array"" : [],
                    ""null array"": [null, null, null],
                    ""bool array"": [true, false, false, true],
                    ""integer array"": [1, -2, 3, -4, 5, 0 ],
                    ""string array"": [""a"", ""b"", ""c"", ""d""]
                }");

            original_json = JsonSerializer.Serialize(document.RootElement);
        };

        Because of = () => recovered_json = JsonSerializer.Serialize(JsonDocument.Parse(content_converter.ToJSON(content_converter.ToBSON(original_json))).RootElement);

        It should_recover_the_same_contents = () => recovered_json.ShouldEqual(original_json);
    }
}