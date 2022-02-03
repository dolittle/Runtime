// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.given;

public class a_converter_and_inputs
{
    protected static Mock<IValueConverter> value_converter;
    protected static ProjectionConverter projection_converter;

    protected static ProjectionState state_to_convert;
    protected static PropertyConversion[] conversions_to_apply;

    Establish context = () =>
    {
        value_converter = new Mock<IValueConverter>(MockBehavior.Strict);
        projection_converter = new ProjectionConverter(value_converter.Object);

        conversions_to_apply = Array.Empty<PropertyConversion>();
    };

    protected static BsonDocument result;
    protected static Exception exception;

    Because of = () => exception = Catch.Exception(() => result = projection_converter.Convert(state_to_convert, conversions_to_apply));
}