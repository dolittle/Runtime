// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Grpc.Core;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Dolittle.Runtime.Services.ReverseCalls.given;

public class all_dependencies
{
    protected static RequestId request_id;
    protected static Mock<IConvertReverseCallMessages<a_message, a_message, object, object, object, object>> message_converter;
    protected static ILoggerFactory logger_factory;
    protected static ILogger logger;
    protected static IMetricsCollector metrics;
    protected static ServerCallContext server_call_context;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        request_id = "request-id";

        message_converter = new Mock<IConvertReverseCallMessages<a_message, a_message, object, object, object, object>>();

        logger_factory = NullLoggerFactory.Instance;
        logger = NullLogger.Instance;

        metrics = Mock.Of<IMetricsCollector>();

        server_call_context = Mock.Of<ServerCallContext>();

        cancellation_token = CancellationToken.None;
    };
}