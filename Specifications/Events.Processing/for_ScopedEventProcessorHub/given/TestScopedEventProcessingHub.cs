// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Execution;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given
{
    public class TestScopedEventProcessingHub : ScopedEventProcessingHub
    {
        public List<CommittedEventStreamWithContext> Processed { get; }

        public List<CommittedEventStreamWithContext> Queued { get; }

        public TestScopedEventProcessingHub(IExecutionContextManager executionContextManager, ILogger logger)
            : base(executionContextManager, logger)
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