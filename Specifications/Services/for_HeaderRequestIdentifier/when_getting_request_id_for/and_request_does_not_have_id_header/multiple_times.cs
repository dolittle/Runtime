// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.when_getting_request_id_for.and_request_does_not_have_id_header
{
    public class multiple_times : given.an_identifier_and_a_call_context
    {
        Establish context = () =>
        {
            call_context.RequestHeaders.Add("X-Request-ID-", "lU6dVasyXMzXUapqTWv");
            call_context.RequestHeaders.Add("Other-Header", "Xq9IxGf6lVbDQiSykaVU");
        };

        static RequestId first_result;
        static RequestId second_result;
        static RequestId third_result;
        Because of = () =>
        {
            first_result = identifier.GetRequestIdFor(call_context);
            second_result = identifier.GetRequestIdFor(call_context);
            third_result = identifier.GetRequestIdFor(call_context);
        };

        It should_return_a_non_empty_request_id_the_first_time = () => first_result.Value.Length.ShouldBeGreaterThan(0);
        It should_return_a_non_empty_request_id_the_second_time = () => second_result.Value.Length.ShouldBeGreaterThan(0);
        It should_return_a_non_empty_request_id_the_third_time = () => third_result.Value.Length.ShouldBeGreaterThan(0);
        It should_return_a_different_id_the_first_and_second_time = () => first_result.ShouldNotEqual(second_result);
        It should_return_a_different_id_the_first_and_third_time = () => first_result.ShouldNotEqual(third_result);
        It should_return_a_different_id_the_second_and_third_time = () => second_result.ShouldNotEqual(third_result);
    }
}
