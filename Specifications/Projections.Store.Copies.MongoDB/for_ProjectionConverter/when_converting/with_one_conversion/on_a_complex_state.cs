// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_one_conversion;

#pragma warning disable CS0618

public class on_a_complex_state : given.a_converter_and_inputs
{
    static BsonValue converted_one;
    static BsonValue converted_two;
    static BsonValue converted_three;
    static BsonValue converted_four;
    static BsonValue converted_five;

    Establish context = () =>
    {
        state_to_convert = @"
            {
                ""people"":
                [
                    {
                        ""name"": ""Albert Einstein"",
                        ""last_released"": ""1905-11-21T00:00:00.000Z"",
                        ""works"":
                        [
                            {
                                ""title"": ""On the Electrodynamics of Moving Bodies"",
                                ""released"": ""1905-09-26T00:00:00.000Z"",
                            },
                            {
                                ""title"": ""Does the Inertia of a Body Depend Upon Its Energy Content?"",
                                ""released"": ""1905-11-21T00:00:00.000Z""
                            }
                        ]
                    },
                    {
                        ""name"": ""Jens Glad Balchen"",
                        ""last_released"": ""1988-01-01T00:00:00.000Z"",
                        ""works"":
                        [
                            {
                                ""title"": ""Reguleringsteknikk"",
                                ""released"": ""1963-01-01T00:00:00.000Z""
                            },
                            {
                                ""title"": ""Process control. Structures and applications"",
                                ""released"": ""1988-01-01T00:00:00.000Z""
                            }
                        ]
                    },
                    {
                        ""name"": ""Einar Ingebrigtsen"",
                        ""last_released"": ""2013-01-01T00:00:00.000Z"",
                        ""works"":
                        [
                            {
                                ""title"": ""SignalR: Real-time Application Development"",
                                ""released"": ""2013-01-01T00:00:00.000Z""
                            }
                        ]
                    }
                ],
            }
        ";
        
        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "people",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "works",
                        ConversionBSONType.None,
                        false,
                        "",
                        new[]
                        {
                            new PropertyConversion(
                                "released",
                                ConversionBSONType.Date,
                                false,
                                "",
                                Array.Empty<PropertyConversion>()),
                        }),
                }),
        };
        
        converted_one = new BsonArray();
        converted_two = new BsonArray();
        converted_three = new BsonArray();
        converted_four = new BsonArray();
        converted_five = new BsonArray();

        value_converter
            .Setup(_ => _.Convert(new BsonString("1905-09-26T00:00:00.000Z"), ConversionBSONType.Date))
            .Returns(converted_one);
        value_converter
            .Setup(_ => _.Convert(new BsonString("1905-11-21T00:00:00.000Z"), ConversionBSONType.Date))
            .Returns(converted_two);
        value_converter
            .Setup(_ => _.Convert(new BsonString("1963-01-01T00:00:00.000Z"), ConversionBSONType.Date))
            .Returns(converted_three);
        value_converter
            .Setup(_ => _.Convert(new BsonString("1988-01-01T00:00:00.000Z"), ConversionBSONType.Date))
            .Returns(converted_four);
        value_converter
            .Setup(_ => _.Convert(new BsonString("2013-01-01T00:00:00.000Z"), ConversionBSONType.Date))
            .Returns(converted_five);
    };

    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_convert_the_first_value = () => value_converter.Verify(_ => _.Convert(new BsonString("1905-09-26T00:00:00.000Z"), ConversionBSONType.Date), Times.Once);
    It should_convert_the_second_value = () => value_converter.Verify(_ => _.Convert(new BsonString("1905-11-21T00:00:00.000Z"), ConversionBSONType.Date), Times.Once);
    It should_convert_the_third_value = () => value_converter.Verify(_ => _.Convert(new BsonString("1963-01-01T00:00:00.000Z"), ConversionBSONType.Date), Times.Once);
    It should_convert_the_fourth_value = () => value_converter.Verify(_ => _.Convert(new BsonString("1988-01-01T00:00:00.000Z"), ConversionBSONType.Date), Times.Once);
    It should_convert_the_fifth_value = () => value_converter.Verify(_ => _.Convert(new BsonString("2013-01-01T00:00:00.000Z"), ConversionBSONType.Date), Times.Once);
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
    It should_have_the_first_converted_value = () => result["people"][0]["works"][0]["released"].ShouldBeTheSameAs(converted_one);
    It should_have_the_second_converted_value = () => result["people"][0]["works"][1]["released"].ShouldBeTheSameAs(converted_two);
    It should_have_the_third_converted_value = () => result["people"][1]["works"][0]["released"].ShouldBeTheSameAs(converted_three);
    It should_have_the_fourth_converted_value = () => result["people"][1]["works"][1]["released"].ShouldBeTheSameAs(converted_four);
    It should_have_the_fifth_converted_value = () => result["people"][2]["works"][0]["released"].ShouldBeTheSameAs(converted_five);
    It should_otherwise_leave_the_document_untouched = () => result.ShouldEqual(
        new BsonDocument("people", new BsonArray(new []
        {
            new BsonDocument(new []
            {
                new BsonElement("name", new BsonString("Albert Einstein")),
                new BsonElement("last_released", new BsonString("1905-11-21T00:00:00.000Z")),
                new BsonElement("works", new BsonArray(new []
                {
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("On the Electrodynamics of Moving Bodies")),
                        new BsonElement("released", new BsonArray()), 
                    }),
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("Does the Inertia of a Body Depend Upon Its Energy Content?")),
                        new BsonElement("released", new BsonArray()), 
                    }),
                }))
            }),
            new BsonDocument(new []
            {
                new BsonElement("name", new BsonString("Jens Glad Balchen")),
                new BsonElement("last_released", new BsonString("1988-01-01T00:00:00.000Z")),
                new BsonElement("works", new BsonArray(new []
                {
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("Reguleringsteknikk")),
                        new BsonElement("released", new BsonArray()), 
                    }),
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("Process control. Structures and applications")),
                        new BsonElement("released", new BsonArray()), 
                    }),
                }))
            }),
            new BsonDocument(new []
            {
                new BsonElement("name", new BsonString("Einar Ingebrigtsen")),
                new BsonElement("last_released", new BsonString("2013-01-01T00:00:00.000Z")),
                new BsonElement("works", new BsonArray(new []
                {
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("SignalR: Real-time Application Development")),
                        new BsonElement("released", new BsonArray()), 
                    }),
                }))
            }),
        }))
    );
}