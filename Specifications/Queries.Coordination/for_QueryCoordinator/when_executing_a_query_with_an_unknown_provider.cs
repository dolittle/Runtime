using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class when_executing_a_query_with_an_unknown_provider : given.a_query_coordinator
    {
        static QueryForUnknownProvider query;
        static PagingInfo paging;
        static QueryResult result;

        Establish context = () =>
        {
            query = new QueryForUnknownProvider();
            query.Query = "something";
            paging = new PagingInfo();
        };

        Because of = async () => {
            result = await coordinator.Execute(query, paging);
        };

        It should_throw_missing_query_provider = () => result.Exception.ShouldBeOfExactType<MissingQueryProvider>();
    }
}
