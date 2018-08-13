using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Relativity.for_EventHorizon.given
{
    public class all_dependencies
    {
        protected static Mock<IGravitationalLens>   lens;
        protected static Mock<ILogger> logger;

        Establish context = () => 
        {
            lens = new Mock<IGravitationalLens>();
            logger = new Mock<ILogger>();
        };
    }
}