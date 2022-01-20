// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Security;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_Extensions;

public class when_converting_claims_back_and_forth
{
    static Claims claims;
    static Claims result;

    Establish context = () =>
    {
        claims = new Claims(new[]
        {
            new Claim("First", "FirstValue", "FirstType"),
            new Claim("Second", "SecondValue", "SecondType")
        });
    };

    Because of = () => result = claims.ToProtobuf().ToClaims();

    It should_be_exactly_the_same = () => result.ShouldEqual(claims);
}