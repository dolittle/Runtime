namespace Dolittle.Runtime.Events.Specs
{
    using System;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Moq;

    public class mocks
    {
        public static Mock<ILogger> a_logger()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(_ => _.Debug(Moq.It.IsAny<string>(),Moq.It.IsAny<string>(),Moq.It.IsAny<int>(),Moq.It.IsAny<string>()))
                                    .Callback<string,string,int,string>((msg,path,line,mbr) => Console.WriteLine(msg));
            return logger;
        }

        public static Mock<IExecutionContextManager> an_execution_context_manager()
        {
            var mgr = new Mock<IExecutionContextManager>();
            return mgr;
        }
    }
}