// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_FilterValidators.given
{
    public class all_dependencies
    {
        protected static Mock<IContainer> container;
        protected static FilterValidators filter_validators;

        Establish context = () =>
        {
            container = new Mock<IContainer>();
            filter_validators = new FilterValidators(container.Object, Mock.Of<ILogger>());
        };
    }
}