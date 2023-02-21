// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.given;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Dolittle.Services.Contracts;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using mother_of_all_dependencies = Dolittle.Runtime.Services.ReverseCalls.given.all_dependencies;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingedConnection.when_pings_are_requested_every_5_seconds.given;

public class all_dependencies : mother_of_all_dependencies
{
    protected static Scenario scenario;
    protected static a_message first_message_with_5_second_pings;

    Establish context = () =>
    {
        first_message_with_5_second_pings = new a_message();
        var first_message_connect_arguments = new object();
        var first_message_arguments_context = new ReverseCallArgumentsContext()
        {
            PingInterval = Duration.FromTimeSpan(TimeSpan.FromSeconds(5)),
        };

        message_converter
            .Setup(_ => _.GetConnectArguments(first_message_with_5_second_pings))
            .Returns(first_message_connect_arguments);
        message_converter
            .Setup(_ => _.GetArgumentsContext(first_message_connect_arguments))
            .Returns(first_message_arguments_context);
    };
}