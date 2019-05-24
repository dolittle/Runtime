/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_ClaimsExtensions
{
    public class when_converting_claim_to_and_from_protobuf
    {
        static Dolittle.Security.Claim original;
        static Claim protobuf;
        static Dolittle.Security.Claim result;

        Establish context = () => original = new Dolittle.Security.Claim("SomeClaim","SomeValue","SomeClaimType");

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToClaim();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}