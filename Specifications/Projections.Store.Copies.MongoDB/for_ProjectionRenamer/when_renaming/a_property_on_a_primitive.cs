// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class a_property_on_a_primitive : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("property", new BsonInt32(32)),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "property",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "nested",
                        ConversionBSONType.None,
                        true,
                        "whatever",
                        Array.Empty<PropertyConversion>()),
                }),
        };
    };

    It should_fail = () => exception.Should().BeOfType<ValueIsNotDocument>();
}