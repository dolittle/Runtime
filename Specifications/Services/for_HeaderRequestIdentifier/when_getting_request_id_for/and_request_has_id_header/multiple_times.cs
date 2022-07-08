// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.when_getting_request_id_for.and_request_has_id_header;

public class multiple_times : given.an_identifier_and_a_call_context
{
    Establish context = () =>
    {
        call_context.RequestHeaders.Add("X-Request-ID", "LcDX3TNcXUObGuBe10fG");
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

    It should_return_the_value_of_the_header_the_first_time = () => first_result.Value.ShouldEqual("LcDX3TNcXUObGuBe10fG");
    It should_return_the_value_of_the_header_the_second_time = () => second_result.Value.ShouldEqual("LcDX3TNcXUObGuBe10fG");
    It should_return_the_value_of_the_header_the_third_time = () => third_result.Value.ShouldEqual("LcDX3TNcXUObGuBe10fG");
}