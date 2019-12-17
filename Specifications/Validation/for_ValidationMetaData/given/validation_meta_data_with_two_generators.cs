// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Validation.MetaData;
using Machine.Specifications;

namespace Dolittle.Specs.Validation.for_ValidationMetaData.given
{
    public class validation_meta_data_with_two_generators : two_generators
    {
        protected static ValidationMetaData validation_meta_data;

        Establish context = () => validation_meta_data = new ValidationMetaData(generators_mock.Object);
    }
}
