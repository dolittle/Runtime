// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static FilterValidators filter_validators;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
            filter_validators = new FilterValidators(type_finder.Object, container.Object, Mock.Of<ILogger>());
        };
    }
}