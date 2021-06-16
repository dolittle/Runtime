// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.when_getting_request_id_for.and_request_does_not_have_id_header
{
    public class once : given.an_identifier_and_a_call_context
    {
        Establish context = () =>
        {
            call_context.RequestHeaders.Add("X-Request-ID-", "JVek8zo52uqFKvAXdHc");
            call_context.RequestHeaders.Add("Other-Header", "TO5KFlZX3WH1vYf6EH16");
        };

        static RequestId result;
        Because of = () => result = identifier.GetRequestIdFor(call_context);

        It should_return_a_non_empty_request_id = () => result.Value.Length.ShouldBeGreaterThan(0);
    }
}
