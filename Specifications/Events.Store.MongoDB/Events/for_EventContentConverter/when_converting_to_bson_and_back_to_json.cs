// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

            original_json = @"
            {
                ""null"": null,

                ""true"": true,
                ""false"": false,

                ""empty string"": """",
                ""short string"": ""some stringy things"",
                ""escaped unicode string"": ""\uA66D"",
                ""unicode string"": ""⍂㈴⍂"",
                ""emoji string"": ""😁🇳🇴🙋🏼‍♀️🙋🏿‍♂️❤️"",

                ""zero integer"": 0,
                ""positive integer"": 123,
                ""negative integer"": -321,
                ""big positive integer"": 9223372036854775807,
                ""big negative integer"": -9223372036854775808,

                ""zero decimal"": 0.0,
                ""positive decimal"": 1.23,
                ""negative decimal"": -3.21,
                ""big positive decimal"": 987654321.12345004,
                ""big negative decimal"": 123456789.98765001,

                ""big positive exponential"": 1.7976931348623157E+308,
                ""big negative exponential"": -1.7976931348623157E+308,
                ""small positive exponential"": 4.9406564584124654E-324,
                ""small negative exponential"": -4.9406564584124654E-324,

                ""date iso"": ""1970-01-01T00:00:00.000Z"",
                ""date iso max"": ""0001-01-01T00:00:00.000Z"",
                ""date iso min"": ""9999-12-31T00:00:00.000Z"",
                ""date dotnet"": ""01/01/1970 00:00:00"",
                ""date dotnet tz"": ""01/01/1970 00:00:00 +00:00"",
                ""date dotnet max"": ""01/01/0001 00:00:00 +00:00"",
                ""date dotnet min"": ""31/12/9999 23:59:59 +00:00"",
                ""date js"": ""Thu Jan 01 1970 01:00:00 GMT+0100 (Central European Standard Time)"",

                ""empty array"" : [],
                ""null array"": [null, null, null],
                ""bool array"": [true, false, false, true],
                ""integer array"": [1, -2, 3, -4, 5, 0 ],
                ""string array"": [""a"", ""b"", ""c"", ""d""],
                ""array array"": [[], [1, 2], [[], [1, 2, [3]]]],
                ""object array"": [{""a"": null}, {""b"": [{ ""c"": [{}]}]}],
                ""mixed array"": [null, 1, false, 1.23, ""c"", [1, 2, ""b""], { ""d"": {}, ""e"": []}],

                ""object"": {
                    ""a"": null,
                    ""b"": false,
                    ""c"": 12.34,
                    ""d"": [1, false, null, ""string"", [], {}],
                    ""e"": { ""a"": false }
                }
            }".FormatJson();
        };

        Because of = () => recovered_json = content_converter.ToJson(content_converter.ToBson(original_json)).FormatJson();

        It should_recover_the_same_contents = () => recovered_json.ShouldEqual(original_json);
    }
}