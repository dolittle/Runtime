// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.when_getting_request_id_for.and_request_has_id_header;

public class once : given.an_identifier_and_a_call_context
{
    Establish context = () =>
    {
        call_context.RequestHeaders.Add("X-Request-ID", "cv8a5w0dt8oHiW2q5NFb");
        call_context.RequestHeaders.Add("X-Request-ID-", "BT0Nr1M2pbWYwIJqJU3");
        call_context.RequestHeaders.Add("Other-Header", "LSJtEzVgVgxoXJHz8MHd");
    };

    static RequestId result;
    Because of = () => result = identifier.GetRequestIdFor(call_context);

    It should_return_the_value_of_the_header = () => result.Value.ShouldEqual("cv8a5w0dt8oHiW2q5NFb");
}