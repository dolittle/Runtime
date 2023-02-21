// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Integration.Tests.Services.given;
using Machine.Specifications;
using Proto;

namespace Integration.Tests.Services.ReverseCalls.when_initiating;

[Tags("IntegrationTest")]
class and_everything_is_ok : given.all_dependencies_and_single_tenant
{
    Establish context = () =>
    {
        setup(
            async (reader, writer, context) => connect_result = await reverse_call_initiator.Connect(reader, writer, context, new basic_with_ping_pong_reverse_call_service_protocol(), context.CancellationToken),
            TimeSpan.FromSeconds(5));
    };

    Because of = () =>
    {
        reverse_call_client.Connect(new RegistrationRequest
        {
            IsValid = true
        }, execution_context, new CancellationTokenSource(500).Token).GetAwaiter().GetResult();
        given.all_dependencies_and_single_tenant.connect_completion_source.Task.GetAwaiter().GetResult();
    };

    It should_not_fail = () => connect_result.Success.ShouldBeTrue();
    It should_return_the_non_failed_arguments = () => connect_result.Result.Item2.Valid.ShouldBeTrue();
    It should_return_a_dispatcher = () => connect_result.Result.Item1.ShouldNotBeNull();
    It should_return_dispatcher_with_the_correct_execution_context = () => connect_result.Result.Item1.ExecutionContext.ShouldEqual(execution_context);
}