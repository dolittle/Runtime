// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Serialization.Protobuf.for_Serializer.given
{
    public class all_dependencies
    {
        protected static Mock<IMessageDescriptions> message_descriptions;
        protected static Mock<IValueConverters> value_converters;

        Establish context = () =>
        {
            message_descriptions = new Mock<IMessageDescriptions>();
            value_converters = new Mock<IValueConverters>();
        };
    }
}