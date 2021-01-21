// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Internal;
using Machine.Specifications;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Specs.Logging.for_InternalLogger.given
{
    public class a_logger_with_one_failing_and_one_successful_writer
    {
        protected static InternalLogger<a_logger_with_one_failing_and_one_successful_writer> logger;
        protected static Mock<ILogMessageWriter> writer_one;
        protected static Mock<ILogMessageWriter> writer_two;

        Establish context = () =>
        {
            writer_one = new Mock<ILogMessageWriter>();
            writer_one.Setup(_ => _.Write(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<object[]>())).Throws(new Exception("Some exception"));
            writer_one.Setup(_ => _.Write(It.IsAny<LogLevel>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>())).Throws(new Exception("Some exception"));
            writer_two = new Mock<ILogMessageWriter>();
            logger = new InternalLogger<a_logger_with_one_failing_and_one_successful_writer>
            {
                LogMessageWriters = new[] { writer_one.Object, writer_two.Object },
            };
        };
    }
}