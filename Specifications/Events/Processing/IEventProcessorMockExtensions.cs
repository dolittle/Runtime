// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Moq;

namespace Dolittle.Runtime.Events.Specs.Processing
{
    public static class IEventProcessorMockExtensions
    {
        public static void ShouldHaveProcessedOnly(this Mock<IEventProcessor> mockProcessor, IEnumerable<CommittedEventEnvelope> @events)
        {
            mockProcessor.Verify(p => p.Process(Moq.It.IsAny<CommittedEventEnvelope>()), Times.Exactly(@events.Count()));
            @events.ForEach(e =>
                mockProcessor.Verify(p => p.Process(e), Times.Once()));
        }

        public static void ShouldNotHaveProcessedAnyEvents(this Mock<IEventProcessor> mockProcessor)
        {
            mockProcessor.Verify(p => p.Process(Moq.It.IsAny<CommittedEventEnvelope>()), Times.Never());
        }
    }
}