/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Google.Protobuf.Collections;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_ClaimsExtensions
{
    public class when_converting_claims_to_and_from_protobuf
    {
        static Dolittle.Security.Claims original;
        static RepeatedField<Claim> protobuf;
        static Dolittle.Security.Claims result;

        Establish context = () => original = new Dolittle.Security.Claims(new[] {
            new Dolittle.Security.Claim("FirstClaim","FirstValue","FirstClaimType"),
            new Dolittle.Security.Claim("SecondClaim","SecondValue","SecondClaimType")
        });
        

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToClaims();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}