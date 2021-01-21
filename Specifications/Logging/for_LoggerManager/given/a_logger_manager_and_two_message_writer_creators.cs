// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Logging.Internal;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Specs.Logging.for_LoggerManager.given
{
    public class a_logger_manager_and_two_message_writer_creators
    {
        protected static ILoggerManager loggerManager;
        protected static Mock<ILogMessageWriter> logMessageWriterOne;
        protected static Mock<ILogMessageWriterCreator> logMessageWriterCreatorOne;
        protected static IList<Type> logMessageWriterCreatorOneCreatedForTypes;
        protected static Mock<ILogMessageWriter> logMessageWriterTwo;
        protected static IList<Type> logMessageWriterCreatorTwoCreatedForTypes;
        protected static Mock<ILogMessageWriterCreator> logMessageWriterCreatorTwo;

        Establish context = () =>
        {
            loggerManager = Activator.CreateInstance(typeof(LoggerManager), true) as ILoggerManager;

            logMessageWriterOne = new Mock<ILogMessageWriter>();
            logMessageWriterCreatorOne = new Mock<ILogMessageWriterCreator>();
            logMessageWriterCreatorOneCreatedForTypes = new List<Type>();
            logMessageWriterCreatorOne.Setup(_ => _.CreateFor(Capture.In(logMessageWriterCreatorOneCreatedForTypes))).Returns(logMessageWriterOne.Object);

            logMessageWriterTwo = new Mock<ILogMessageWriter>();
            logMessageWriterCreatorTwo = new Mock<ILogMessageWriterCreator>();
            logMessageWriterCreatorTwoCreatedForTypes = new List<Type>();
            logMessageWriterCreatorTwo.Setup(_ => _.CreateFor(Capture.In(logMessageWriterCreatorTwoCreatedForTypes))).Returns(logMessageWriterTwo.Object);
        };
    }
}