// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class a_nested_property_and_its_parent : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("parent", new BsonDocument(new[]
            {
                new BsonElement("nested", new BsonString("some string")),
            })),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "parent",
                ConversionBSONType.None,
                true,
                "moved parent",
                new[]
                {
                    new PropertyConversion(
                        "nested",
                        ConversionBSONType.None,
                        true,
                        "moved nested",
                        Array.Empty<PropertyConversion>()),
                }),
        };
    };

    It should_return_the_correct_document = () => result.ShouldEqual(new BsonDocument(new[]
    {
        new BsonElement("moved parent", new BsonDocument(new[]
        {
            new BsonElement("moved nested", new BsonString("some string")),
        })),
    }));
}