// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Internal;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Specs.Logging.for_UnknownLogger
{
    public class when_writing_a_log_message
    {
        static string message;
        static object[] arguments;
        static Mock<ILogMessageWriter> writer;
        static UnknownLogger logger;

        Establish context = () =>
        {
            message = "Some message";
            arguments = new object[] { "some arguments", 1 };
            writer = new Mock<ILogMessageWriter>();
            logger = new UnknownLogger
            {
                LogMessageWriters = new[] { writer.Object }
            };
        };

        Because of = () => logger.Information(message, arguments);

        It should_prepend_the_message_with_extra_text = () => writer.Verify(_ => _.Write(LogLevel.Info, Moq.It.Is<string>(writtenMessage => writtenMessage.Length > message.Length), arguments), Times.Once());
        It should_end_the_message_with_the_original_text = () => writer.Verify(_ => _.Write(LogLevel.Info, Moq.It.Is<string>(writtenMessage => writtenMessage.EndsWith(message)), arguments), Times.Once());
    }
}