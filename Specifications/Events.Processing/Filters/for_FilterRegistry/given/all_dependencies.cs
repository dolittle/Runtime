// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterRegistry.given
{
    public class all_dependencies
    {
        protected static Mock<IFilterValidators> validators;
        protected static Mock<IContainer> container;
        protected static IFilterRegistry filter_registry;

        Establish context = () =>
        {
            validators = new Mock<IFilterValidators>();
            container = new Mock<IContainer>();
            filter_registry = new FilterRegistry(validators.Object, container.Object, Mock.Of<ILogger>());
        };
    }
}