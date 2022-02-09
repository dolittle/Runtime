// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.given;

public class a_converter_and_inputs
{
    protected static Mock<IValueConverter> value_converter;
    protected static Mock<IPropertyRenamer> property_renamer;
    protected static ProjectionConverter projection_converter;

    protected static ProjectionState state_to_convert;
    protected static PropertyConversion[] conversions_to_apply;

    Establish context = () =>
    {
        value_converter = new Mock<IValueConverter>(MockBehavior.Strict);
        property_renamer = new Mock<IPropertyRenamer>();
        property_renamer
            .Setup(_ => _.RenamePropertiesIn(It.IsAny<BsonDocument>(), It.IsAny<PropertyConversion[]>()))
            .Returns<BsonDocument, PropertyConversion[]>((document, _) => document);

        projection_converter = new ProjectionConverter(value_converter.Object, property_renamer.Object);

        conversions_to_apply = Array.Empty<PropertyConversion>();
    };

    protected static BsonDocument result;
    protected static Exception exception;

    Because of = () => exception = Catch.Exception(() => result = projection_converter.Convert(state_to_convert, conversions_to_apply));
}