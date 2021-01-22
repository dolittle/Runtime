// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Internal;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Specs.Logging.for_InternalLogger.given
{
    public class a_logger_with_two_writers
    {
        protected static InternalLogger<a_logger_with_two_writers> logger;
        protected static Mock<ILogMessageWriter> writer_one;
        protected static Mock<ILogMessageWriter> writer_two;

        Establish context = () =>
        {
            writer_one = new Mock<ILogMessageWriter>();
            writer_two = new Mock<ILogMessageWriter>();
            logger = new InternalLogger<a_logger_with_two_writers>
            {
                LogMessageWriters = new[] { writer_one.Object, writer_two.Object },
            };
        };
    }
}