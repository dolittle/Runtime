/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Globalization;
using Dolittle.Applications;
using Dolittle.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_ArtifactExtensions
{
    public class when_converting_execution_context_to_and_from_protobuf
    {
        static Dolittle.Execution.ExecutionContext original;
        static ExecutionContext protobuf;
        static Dolittle.Execution.ExecutionContext result;

        Establish context = () => original = new Dolittle.Execution.ExecutionContext(
            Application.New(),
            BoundedContext.New(),
            Guid.NewGuid(),
            "Development",
            CorrelationId.New(),
            new Dolittle.Security.Claims(
                new[] {
                    new Dolittle.Security.Claim("FirstClaim","FirstValue","FirstClaimType"),
                    new Dolittle.Security.Claim("SecondClaim","SecondValue","SecondClaimType")
                }
            ),
            CultureInfo.InvariantCulture
        );

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToExecutionContext();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}