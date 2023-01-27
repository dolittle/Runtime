// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.without_conversions;

#pragma warning disable CS0618

public class a_complex_state : given.a_converter_and_inputs
{
    Establish context = () => state_to_convert = @"
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

    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_return_the_correct_document = () => result.Should().BeEquivalentTo(
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
                        new BsonElement("released", new BsonString("1905-09-26T00:00:00.000Z")),
                    }),
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("Does the Inertia of a Body Depend Upon Its Energy Content?")),
                        new BsonElement("released", new BsonString("1905-11-21T00:00:00.000Z")),
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
                        new BsonElement("released", new BsonString("1963-01-01T00:00:00.000Z")),
                    }),
                    new BsonDocument(new []
                    {
                        new BsonElement("title", new BsonString("Process control. Structures and applications")),
                        new BsonElement("released", new BsonString("1988-01-01T00:00:00.000Z")),
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
                        new BsonElement("released", new BsonString("2013-01-01T00:00:00.000Z")),
                    }),
                }))
            }),
        }))
    );
}