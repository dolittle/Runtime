namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given
{
    using System.Collections.Generic;
    using System.Globalization;
    using Dolittle.Applications;
    using Dolittle.Execution;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using Moq;

    public class TestScopedEventProcessingHub : ScopedEventProcessingHub
    {
        public List<CommittedEventStreamWithContext> Processed { get; }
        public List<CommittedEventStreamWithContext> Queued { get; }

        public TestScopedEventProcessingHub(ILogger logger, IExecutionContextManager executionContextManager) : base(logger, executionContextManager)
        {
            Queued = new List<CommittedEventStreamWithContext>();
            Processed = new List<CommittedEventStreamWithContext>();
        }

        protected override void Enqueue(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            Queued.Add(committedEventStreamWithContext);
            base.Enqueue(committedEventStreamWithContext);
        }

        protected override void ProcessStream(CommittedEventStreamWithContext committedEventStreamWithContext)
        {
            Processed.Add(committedEventStreamWithContext);
        }
    }
}