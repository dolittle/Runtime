// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Serialization.Protobuf.for_ValueConverters.given;

public class all_dependencies
{
    protected static Mock<IEnumerable<IValueConverter>> value_converter_instances;
    Establish context = () => value_converter_instances = new Mock<IEnumerable<IValueConverter>>();
}