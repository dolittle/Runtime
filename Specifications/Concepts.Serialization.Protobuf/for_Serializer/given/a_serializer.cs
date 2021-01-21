// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Serialization.Protobuf;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Concepts.Serialization.Protobuf.for_Serializer.given
{
    public class a_serializer : all_dependencies
    {
        protected static Serializer serializer;
        protected static ConceptConverter concept_value_converter;

        Establish context = () =>
        {
            concept_value_converter = new ConceptConverter();
            var converters = new List<IValueConverter>(new[]
            {
                concept_value_converter
            });

            var value_converters = new Mock<IValueConverters>();
            value_converters.Setup(_ => _.CanConvert(Moq.It.IsAny<Type>())).Returns(true);
            value_converters.Setup(_ => _.GetConverterFor(Moq.It.IsAny<Type>())).Returns(concept_value_converter);
            serializer = new Serializer(message_descriptions.Object, value_converters.Object);
        };
    }
}