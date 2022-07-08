// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.given;

public class an_identifier_and_a_call_context
{
    protected static HeaderRequestIdentifier identifier;
    protected static ServerCallContext call_context;

    Establish context = () =>
    {
        identifier = new HeaderRequestIdentifier();

        call_context = new MockServerCallContext();
    };
}