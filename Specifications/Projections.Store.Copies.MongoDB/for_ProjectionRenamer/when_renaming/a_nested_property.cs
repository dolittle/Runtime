// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class a_nested_property : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("first level", new BsonDocument(new[]
            {
                new BsonElement("second level", new BsonDocument(new[]
                {
                    new BsonElement("third level", new BsonDocument(new[]
                    {
                        new BsonElement("original", new BsonString("I will be moved")),
                    })),
                })),
            })),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "first level",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "second level",
                        ConversionBSONType.None,
                        false,
                        "",
                        new[]
                        {
                            new PropertyConversion(
                                "third level",
                                ConversionBSONType.None,
                                false,
                                "",
                                new[]
                                {
                                    new PropertyConversion(
                                        "original",
                                        ConversionBSONType.None,
                                        true,
                                        "moved",
                                        Array.Empty<PropertyConversion>()),
                                }),
                        }),
                }),
        };
    };

    It should_return_the_correct_document = () => result.Should().BeEquivalentTo(new BsonDocument(new[]
    {
        new BsonElement("first level", new BsonDocument(new[]
        {
            new BsonElement("second level", new BsonDocument(new[]
            {
                new BsonElement("third level", new BsonDocument(new[]
                {
                    new BsonElement("moved", new BsonString("I will be moved")),
                })),
            })),
        })),
    }));
}