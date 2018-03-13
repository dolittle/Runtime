using Dolittle.ReadModels;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class ReadModelWithString : IReadModel
    {
        public string Content { get; set; }
    }
}
