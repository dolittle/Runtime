// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class to_a_property_that_exists : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("existing", new BsonArray()),
            new BsonElement("to_move", new BsonArray()),
        });

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "to_move",
                ConversionBSONType.None,
                true,
                "existing",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_fail = () => exception.Should().BeOfType<DocumentAlreadyContainsProperty>();
}