// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Applications;
using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_ArtifactExtensions
{
    public class when_converting_execution_context_to_and_from_protobuf
    {
        static Execution.ExecutionContext original;
        static ExecutionContext protobuf;
        static Execution.ExecutionContext result;

        Establish context = () => original = new Execution.ExecutionContext(
            Application.New(),
            BoundedContext.New(),
            Guid.NewGuid(),
            "Development",
            Execution.CorrelationId.New(),
            new Security.Claims(
                new[]
                {
                    new Security.Claim("FirstClaim", "FirstValue", "FirstClaimType"),
                    new Security.Claim("SecondClaim", "SecondValue", "SecondClaimType")
                }),
            CultureInfo.InvariantCulture);

        Because of = () =>
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToExecutionContext();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}