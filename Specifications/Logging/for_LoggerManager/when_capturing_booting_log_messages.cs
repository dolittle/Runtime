// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Specs.Logging.for_LoggerManager.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Specs.Logging.for_LoggerManager
{
    public class when_capturing_booting_log_messages : given.a_logger_manager_and_two_message_writer_creators
    {
        static ILogger stringLogger;
        static ILogger intLogger;
        static string firstMessage;
        static object[] firstArguments;
        static string secondMessage;
        static object[] secondArguments;
        static string thirdMessage;
        static object[] thirdArguments;

        static IList<Type> writtenLogTypes;
        static IList<LogLevel> writtenLogLevels;
        static IList<string> writtenLogMessages;
        static IList<object[]> writtenLogArguments;
        static Mock<ILogMessageWriterCreator> logMessageWriterCreatorThree;

        Establish context = () =>
        {
            firstMessage = "First message";
            firstArguments = new object[] { "arguments", 1 };
            secondMessage = "Second message";
            secondArguments = new object[] { 2, "other args" };
            thirdMessage = "Third message";
            thirdArguments = new object[] { null, 3 };

            writtenLogTypes = new List<Type>();
            writtenLogLevels = new List<LogLevel>();
            writtenLogMessages = new List<string>();
            writtenLogArguments = new List<object[]>();
            logMessageWriterCreatorThree = new Mock<ILogMessageWriterCreator>();
            logMessageWriterCreatorThree.Setup(_ => _.CreateFor(Moq.It.IsAny<Type>())).Returns((Type type) =>
            {
                var logWriter = new Mock<ILogMessageWriter>();
                logWriter.Setup(_ => _.Write(Moq.It.IsAny<LogLevel>(), Moq.It.IsAny<string>(), Moq.It.IsAny<object[]>()))
                    .Callback((LogLevel logLevel, string message, object[] arguments) =>
                    {
                        writtenLogTypes.Add(type);
                        writtenLogLevels.Add(logLevel);
                        writtenLogMessages.Add(message);
                        writtenLogArguments.Add(arguments);
                    });
                return logWriter.Object;
            });
        };

        Because of = () =>
        {
            stringLogger = loggerManager.CreateLogger<string>();
            intLogger = loggerManager.CreateLogger(typeof(int));

            stringLogger.Information(firstMessage, firstArguments);
            intLogger.Warning(secondMessage, secondArguments);

            loggerManager.AddLogMessageWriterCreators(new[]
            {
                logMessageWriterCreatorOne.Object,
                logMessageWriterCreatorTwo.Object,
                logMessageWriterCreatorThree.Object
            });

            intLogger.Trace(thirdMessage, thirdArguments);
        };

        It should_replace_the_log_message_writers = () =>
        {
            stringLogger.GetLogMessageWriters().Length.ShouldEqual(3);
            intLogger.GetLogMessageWriters().Length.ShouldEqual(3);
        };

        It should_write_all_log_messages_in_order = () =>
        {
            writtenLogTypes.ShouldContainOnly(typeof(string), typeof(int), typeof(int));
            writtenLogLevels.ShouldContainOnly(LogLevel.Info, LogLevel.Warning, LogLevel.Trace);
            writtenLogMessages.ShouldContainOnly(firstMessage, secondMessage, thirdMessage);
            writtenLogArguments.ShouldContainOnly(firstArguments, secondArguments, thirdArguments);
        };
    }
}