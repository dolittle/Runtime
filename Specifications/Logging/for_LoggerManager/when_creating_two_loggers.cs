// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Specs.Logging.for_LoggerManager.given;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Specs.Logging.for_LoggerManager
{
    public class when_creating_two_loggers : given.a_logger_manager_and_two_message_writer_creators
    {
        static ILogger stringLogger;
        static ILogger intLogger;

        Establish context = () => loggerManager.AddLogMessageWriterCreators(new[]
        {
            logMessageWriterCreatorOne.Object,
            logMessageWriterCreatorTwo.Object
        });

        Because of = () =>
        {
            stringLogger = loggerManager.CreateLogger<string>();
            intLogger = loggerManager.CreateLogger(typeof(int));
        };

        It should_create_a_string_logger_with_the_correct_type = () => stringLogger.ShouldBeAssignableTo<ILogger<string>>();

        It should_create_an_int_logger_with_the_correct_type = () => intLogger.ShouldBeAssignableTo<ILogger<int>>();

        It should_create_loggers_with_two_writers = () =>
        {
            stringLogger.GetLogMessageWriters().Length.ShouldEqual(2);
            intLogger.GetLogMessageWriters().Length.ShouldEqual(2);
        };

        It should_create_writers_for_both_types = () =>
        {
            logMessageWriterCreatorOneCreatedForTypes.ShouldContainOnly(typeof(int), typeof(string));
            logMessageWriterCreatorOne.Verify(_ => _.CreateFor(typeof(int)), Times.Once());
            logMessageWriterCreatorOne.Verify(_ => _.CreateFor(typeof(string)), Times.Once());
            logMessageWriterCreatorOne.VerifyNoOtherCalls();

            logMessageWriterCreatorTwoCreatedForTypes.ShouldContainOnly(typeof(int), typeof(string));
            logMessageWriterCreatorTwo.Verify(_ => _.CreateFor(typeof(int)), Times.Once());
            logMessageWriterCreatorTwo.Verify(_ => _.CreateFor(typeof(string)), Times.Once());
            logMessageWriterCreatorTwo.VerifyNoOtherCalls();
        };
    }
}