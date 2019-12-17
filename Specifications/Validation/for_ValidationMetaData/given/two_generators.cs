// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Validation.MetaData;
using Machine.Specifications;

namespace Dolittle.Specs.Validation.for_ValidationMetaData.given
{
    public class two_generators : all_dependencies
    {
        protected static first_generator first_generator;
        protected static second_generator second_generator;

        Establish context = () =>
        {
            first_generator = new first_generator();
            second_generator = new second_generator();

            generators_mock.Setup(g => g.GetEnumerator()).Returns(new ICanGenerateValidationMetaData[]
            {
                first_generator,
                second_generator
            }.ToList<ICanGenerateValidationMetaData>().GetEnumerator());
        };
    }
}
