// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptors.given
{
    public class no_query_validation_descriptors : all_dependencies
    {
        protected static QueryValidationDescriptors descriptors;

        Establish context = () =>
        {
            type_finder.Setup(t => t.FindMultiple(typeof(QueryValidationDescriptorFor<>))).Returns(Array.Empty<Type>());
            descriptors = new QueryValidationDescriptors(type_finder.Object, container.Object);
        };
    }
}
