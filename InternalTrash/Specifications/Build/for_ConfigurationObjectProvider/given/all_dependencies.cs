// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Build.for_ConfigurationObjectProvider.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static GetContainer get_container;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
            get_container = () => container.Object;
        };
    }
}