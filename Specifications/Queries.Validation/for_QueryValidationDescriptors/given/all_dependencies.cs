// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationDescriptors.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
        };
    }
}
