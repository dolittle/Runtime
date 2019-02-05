using System.Threading.Tasks;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class AsyncQueryForKnownProvider : IQuery
    {
        public QueryType QueryToReturn;
        public bool QueryPropertyCalled;
        public Task<QueryType> Query
        {
            get
            {
                QueryPropertyCalled = true;
                return Task.FromResult(QueryToReturn);
            }
        }
    }
}
