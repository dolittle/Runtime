// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class two_properties_to_the_same_property : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("first", new BsonArray()),
            new BsonElement("second", new BsonArray()),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "first",
                ConversionBSONType.None,
                true,
                "same",
                Array.Empty<PropertyConversion>()),
            new PropertyConversion(
                "second",
                ConversionBSONType.None,
                true,
                "same",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_fail = () => exception.ShouldBeOfExactType<DocumentAlreadyContainsProperty>();
}