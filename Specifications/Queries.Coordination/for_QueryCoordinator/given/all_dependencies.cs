using doLittle.DependencyInversion;
using doLittle.Queries.Security;
using doLittle.Queries.Validation;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Queries.Coordination.Specs.for_QueryCoordinator.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static Mock<IFetchingSecurityManager> fetching_security_manager;
        protected static Mock<IReadModelFilters> read_model_filters;
        protected static Mock<IQueryValidator> query_validator;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
            fetching_security_manager = new Mock<IFetchingSecurityManager>();
            read_model_filters = new Mock<IReadModelFilters>();
            query_validator = new Mock<IQueryValidator>();
        };
    }
}
