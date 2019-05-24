/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_ArtifactExtensions
{
    public class when_converting_original_context_to_and_from_protobuf
    {
        static OriginalContext original;
        static Runtime.Grpc.Interaction.Protobuf.OriginalContext protobuf;
        static OriginalContext result;

        Establish context = () => original = new OriginalContext(
            Application.New(),
            BoundedContext.New(),
            Guid.NewGuid(),
            "Development",
            new Dolittle.Security.Claims(
                new[] {
                    new Dolittle.Security.Claim("FirstClaim","FirstValue","FirstClaimType"),
                    new Dolittle.Security.Claim("SecondClaim","SecondValue","SecondClaimType")
                }
            ),
            new CommitSequenceNumber(42)
        );

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToOriginalContext();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}