// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Specs.for_ReadModelFilters.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_discoverer;
        protected static Mock<IContainer> container;

        Establish context = () =>
        {
            type_discoverer = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
        };
    }
}
