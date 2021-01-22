// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Serialization.Protobuf.for_ValueConverters.given
{
    public class two_value_converters : all_dependencies
    {
        protected static Mock<IValueConverter> first_value_converter;
        protected static Mock<IValueConverter> second_value_converter;
        protected static ValueConverters value_converters;

        Establish context = () =>
        {
            first_value_converter = new Mock<IValueConverter>();
            second_value_converter = new Mock<IValueConverter>();

            value_converter_instances.Setup(_ => _.GetEnumerator()).Returns(new List<IValueConverter>(new[]
            {
                first_value_converter.Object,
                second_value_converter.Object
            }).GetEnumerator());

            value_converters = new ValueConverters(value_converter_instances.Object);
        };
    }
}