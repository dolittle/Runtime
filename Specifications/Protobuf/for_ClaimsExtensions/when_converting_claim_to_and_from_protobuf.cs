// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_ClaimsExtensions
{
    public class when_converting_claim_to_and_from_protobuf
    {
        static Security.Claim original;
        static Claim protobuf;
        static Security.Claim result;

        Establish context = () => original = new Security.Claim("SomeClaim", "SomeValue", "SomeClaimType");

        Because of = () =>
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToClaim();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}