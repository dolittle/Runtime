// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Security;
using Machine.Specifications;
using ExecutionContextContract = Dolittle.Execution.Contracts.ExecutionContext;

namespace Dolittle.Runtime.Protobuf.for_Extensions;

public class when_converting_execution_context_to_protobuf
{
    static ExecutionContext execution_context;
    static ExecutionContextContract result;
    static Versioning.Version version;

    Establish context = () =>
    {
        version = new Versioning.Version(1, 2, 3, 1, "test");
        execution_context = new ExecutionContext(
            Guid.NewGuid(),
            Guid.NewGuid(),
            version,
            Execution.Environment.Development,
            Guid.NewGuid(),
            new Claims(new[]
            {
                new Claim("First", "FirstValue", "FirstType"),
                new Claim("Second", "SecondValue", "SecondType")
            }),
            CultureInfo.InvariantCulture);
    };

    Because of = () => result = execution_context.ToProtobuf();

    It should_hold_the_correct_microservice = () => result.MicroserviceId.ToGuid().ShouldEqual(execution_context.Microservice.Value);
    It should_hold_the_correct_tenant = () => result.TenantId.ToGuid().ShouldEqual(execution_context.Tenant.Value);
    It should_hold_the_correct_version = () => result.Version.ToVersion().ShouldEqual(execution_context.Version);
    It should_hold_the_correct_correlation_id = () => result.CorrelationId.ToGuid().ShouldEqual(execution_context.CorrelationId.Value);
    It should_hold_the_correct_claims = () => result.Claims.ToClaims().ShouldEqual(execution_context.Claims);
}