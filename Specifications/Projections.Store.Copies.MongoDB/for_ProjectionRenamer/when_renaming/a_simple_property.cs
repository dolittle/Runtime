// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class a_simple_property : given.a_renamer_and_inputs
{
    static BsonString string_to_rename;
    static BsonString string_to_leave;
    
    Establish context = () =>
    {
        string_to_rename = new BsonString("I will be moved");
        string_to_leave = new BsonString("I will not be moved");
        
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("some_string", string_to_rename),
            new BsonElement("other_string", string_to_leave),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "some_string",
                ConversionBSONType.None,
                true,
                "another_string",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_have_the_renamed_property = () => result["another_string"].ShouldBeTheSameAs(string_to_rename);
    It should_not_move_the_other_property = () => result["other_string"].ShouldBeTheSameAs(string_to_leave);
    It should_not_have_the_old_property = () => result.Contains("some_string").ShouldBeFalse();
}