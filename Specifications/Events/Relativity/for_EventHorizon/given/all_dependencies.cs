using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon.given
{
    public class all_dependencies
    {
        protected static Mock<ILogger> logger;

        Establish context = () => 
        {
            logger = new Mock<ILogger>();
        };
    }
}