// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Services.ReverseCalls.given
{
    public class all_dependencies
    {
        protected static RequestId request_id;
        protected static Mock<IConvertReverseCallMessages<a_message, a_message, object, object, object, object>> message_converter;
        protected static ILoggerFactory logger_factory;
        protected static ILogger logger;
        protected static IMetricsCollector metrics;
        protected static CancellationToken cancellation_token;

        Establish context = () =>
        {
            request_id = "request-id";
            message_converter = new Mock<IConvertReverseCallMessages<a_message, a_message, object, object, object, object>>();
            logger_factory = Mock.Of<ILoggerFactory>();
            logger = Mock.Of<ILogger>();
            metrics = Mock.Of<IMetricsCollector>();
            cancellation_token = CancellationToken.None;
        };
    }
}