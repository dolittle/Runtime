// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class two_properties : given.a_renamer_and_inputs
{
    static BsonValue first_value;
    static BsonValue second_value;

    Establish context = () =>
    {
        first_value = new BsonArray();
        second_value = new BsonArray();

        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("first", first_value),
            new BsonElement("second", second_value),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "first",
                ConversionBSONType.None,
                true,
                "new first",
                Array.Empty<PropertyConversion>()),
            new PropertyConversion(
                "second",
                ConversionBSONType.None,
                true,
                "new second",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_have_moved_the_first_property = () => result["new first"].ShouldBeTheSameAs(first_value);
    It should_have_moved_the_second_property = () => result["new second"].ShouldBeTheSameAs(second_value);
    It should_not_have_the_first_property_anymore = () => result.Contains("first").Should().BeFalse();
    It should_not_have_the_second_property_anymore = () => result.Contains("second").Should().BeFalse();
}