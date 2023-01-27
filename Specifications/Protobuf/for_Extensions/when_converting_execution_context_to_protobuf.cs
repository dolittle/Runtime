// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.Runtime.Execution;
using FluentAssertions;
using Machine.Specifications;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using ExecutionContextContract = Dolittle.Execution.Contracts.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Protobuf.for_Extensions;

public class when_converting_execution_context_to_protobuf
{
    static ExecutionContext execution_context;
    static ExecutionContextContract result;
    static Version version;

    Establish context = () =>
    {
        version = new Version(1, 2, 3, 1, "test");
        execution_context = new ExecutionContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            version,
            Environment.Development,
            Guid.NewGuid(),
            ActivitySpanId.CreateRandom(),
            new Claims(new[]
            {
                new Claim("First", "FirstValue", "FirstType"),
                new Claim("Second", "SecondValue", "SecondType")
            }),
            CultureInfo.InvariantCulture);
    };

    Because of = () => result = execution_context.ToProtobuf();

    It should_hold_the_correct_microservice = () => result.MicroserviceId.ToGuid().Should().Be(execution_context.Microservice.Value);
    It should_hold_the_correct_tenant = () => result.TenantId.ToGuid().Should().Be(execution_context.Tenant.Value);
    It should_hold_the_correct_version = () => result.Version.ToVersion().Should().Be(execution_context.Version);
    It should_hold_the_correct_correlation_id = () => result.CorrelationId.ToGuid().Should().Be(execution_context.CorrelationId.Value);
    It should_hold_the_correct_claims = () => result.Claims.ToClaims().Should().BeEquivalentTo(execution_context.Claims);
    It should_hold_the_correct_span_id = () => ActivitySpanId.CreateFromBytes(result.SpanId.Span).Should().Be(execution_context.SpanId!.Value);
}