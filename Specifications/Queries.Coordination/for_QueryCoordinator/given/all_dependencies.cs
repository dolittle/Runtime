// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Queries.Security;
using Dolittle.Queries.Validation;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static Mock<IFetchingSecurityManager> fetching_security_manager;
        protected static Mock<IReadModelFilters> read_model_filters;
        protected static Mock<IQueryValidator> query_validator;
        protected static ILogger logger;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
            fetching_security_manager = new Mock<IFetchingSecurityManager>();
            read_model_filters = new Mock<IReadModelFilters>();
            query_validator = new Mock<IQueryValidator>();
            logger = Mock.Of<ILogger>();
        };
    }
}
