// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Logging.for_LoggerManager
{
    public class when_creating_a_logger_for_the_same_type_twice : given.a_logger_manager_and_two_message_writer_creators
    {
        static ILogger first_logger;
        static ILogger second_logger;

        Because of = () =>
        {
            first_logger = loggerManager.CreateLogger<string>();
            second_logger = loggerManager.CreateLogger(typeof(string));
        };

        It should_return_the_same_logger_instance = () => first_logger.ShouldBeTheSameAs(second_logger);
    }
}