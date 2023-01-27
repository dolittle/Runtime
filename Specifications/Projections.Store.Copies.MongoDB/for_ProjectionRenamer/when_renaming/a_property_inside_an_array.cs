// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class a_property_inside_an_array : given.a_renamer_and_inputs
{
    static BsonString string_one;
    static BsonString string_two;
    static BsonString string_three;
    static BsonArray array_with_objects;

    Establish context = () =>
    {
        string_one = new BsonString("string one");
        string_two = new BsonString("string two");
        string_three = new BsonString("string three");
        
        array_with_objects = new BsonArray()
        {
            new BsonDocument(new []
            {
                new BsonElement("property_to_move", string_one),
                new BsonElement("property_to_keep", new BsonBoolean(true)),
            }),
            new BsonDocument(new []
            {
                new BsonElement("property_to_move", string_two),
                new BsonElement("property_to_keep", new BsonBoolean(true)),
            }),
            new BsonDocument(new []
            {
                new BsonElement("property_to_move", string_three),
                new BsonElement("property_to_keep", new BsonBoolean(false)),
            }),
        };
        
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("document", new BsonDocument(new[]
            {
                new BsonElement("array", array_with_objects),
            })),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "document",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "array",
                        ConversionBSONType.None,
                        false,
                        "",
                        new[]
                        {
                            new PropertyConversion(
                                "property_to_move",
                                ConversionBSONType.None,
                                true,
                                "moved_property",
                                Array.Empty<PropertyConversion>()),
                        }),
                }),
        };
    };

    It should_return_the_correct_document = () => result.Should().BeEquivalentTo(new BsonDocument(new[]
    {
        new BsonElement("document", new BsonDocument(new[]
        {
            new BsonElement("array", new BsonArray()
            {
                new BsonDocument(new[]
                {
                    new BsonElement("property_to_keep", new BsonBoolean(true)),
                    new BsonElement("moved_property", new BsonString("string one")),
                }),
                new BsonDocument(new[]
                {
                    new BsonElement("property_to_keep", new BsonBoolean(true)),
                    new BsonElement("moved_property", new BsonString("string two")),
                }),
                new BsonDocument(new[]
                {
                    new BsonElement("property_to_keep", new BsonBoolean(false)),
                    new BsonElement("moved_property", new BsonString("string three")),
                }),
            })
        }))
    }));
    It should_not_create_a_new_array = () => result["document"]["array"].ShouldBeTheSameAs(array_with_objects);
    It should_not_change_the_first_string = () => result["document"]["array"][0]["moved_property"].ShouldBeTheSameAs(string_one);
    It should_not_change_the_second_string = () => result["document"]["array"][1]["moved_property"].ShouldBeTheSameAs(string_two);
    It should_not_change_the_third_string = () => result["document"]["array"][2]["moved_property"].ShouldBeTheSameAs(string_three);
}