// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class an_array : given.a_renamer_and_inputs
{
    static BsonArray array_to_move;
    
    Establish context = () =>
    {
        array_to_move = new BsonArray()
        {
            new BsonString("Hello World"),
            new BsonInt32(42),
        };
        
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("array", array_to_move),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "array",
                ConversionBSONType.None,
                true,
                "moved_array",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_have_the_renamed_property = () => result["moved_array"].ShouldBeTheSameAs(array_to_move);
    It should_not_have_the_old_property = () => result.Contains("array").ShouldBeFalse();
}