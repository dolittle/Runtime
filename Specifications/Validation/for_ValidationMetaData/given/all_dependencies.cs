// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Types;
using Dolittle.Validation.MetaData;
using Machine.Specifications;
using Moq;

namespace Dolittle.Specs.Validation.for_ValidationMetaData.given
{
    public class all_dependencies
    {
        protected static Mock<IInstancesOf<ICanGenerateValidationMetaData>> generators_mock;

        Establish context = () => generators_mock = new Mock<IInstancesOf<ICanGenerateValidationMetaData>>();
    }
}
