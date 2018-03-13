namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class QueryForGenericKnownType : IQuery
    {
        public GenericKnownType<object> QueryToReturn;
        public GenericKnownType<object> Query
        {
            get
            {
                return QueryToReturn;
            }
        }
    }
}
