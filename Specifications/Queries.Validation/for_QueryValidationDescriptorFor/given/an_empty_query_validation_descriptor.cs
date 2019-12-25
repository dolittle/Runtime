// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptorFor.given
{
    public class an_empty_query_validation_descriptor
    {
        protected static QueryValidationDescriptorFor<SomeQuery> descriptor;

        Establish context = () => descriptor = new QueryValidationDescriptorFor<SomeQuery>();
    }
}
