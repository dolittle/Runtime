using System.Collections.ObjectModel;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.Definition;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Projections.for_CompareProjectionDefinitionsForAllTenants.given
{
    public class all_dependencies
    {
        protected static CompareProjectionDefinitionsForAllTenants comparer;
        protected static Mock<ITenants> tenants;
        protected static Mock<IProjectionDefinitions> definitions;
        Establish context = () =>
        {
            tenants = new Mock<ITenants>();
            definitions = new Mock<IProjectionDefinitions>();
            tenants.Setup(_ => _.All).Returns(new ObservableCollection<TenantId>(new[] { TenantId.Development }));

            comparer = new CompareProjectionDefinitionsForAllTenants(new PerformActionOnAllTenants(tenants.Object, Mock.Of<IExecutionContextManager>()), () => definitions.Object);
        };
    }
}
