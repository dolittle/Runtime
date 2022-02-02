// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_multiple_conversions;

public class on_a_complex_state : given.a_converter_and_inputs
{
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
        
        conversions_to_apply.Add("people.name", ConversionBSONType.Date);
        conversions_to_apply.Add("people.works.released", ConversionBSONType.Date);

        value_converter
            .Setup(_ => _.Convert(Moq.It.IsAny<BsonString>(), ConversionBSONType.Date))
            .Returns(new BsonArray());
    };

    static BsonDocument result;

    Because of = () => result = projection_converter.Convert(state_to_convert, conversions_to_apply);

    It should_call_the_converter_eight_times = () => value_converter.Verify(_ => _.Convert(Moq.It.IsAny<BsonString>(), ConversionBSONType.Date), Times.Exactly(8));
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
    It should_return_the_correct_document = () => result.ShouldEqual(
        new BsonDocument("people", new BsonArray(new []
        {
            new BsonDocument(new []
            {
                new BsonElement("name", new BsonArray()), 
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
                new BsonElement("name", new BsonArray()), 
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
                new BsonElement("name", new BsonArray()), 
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