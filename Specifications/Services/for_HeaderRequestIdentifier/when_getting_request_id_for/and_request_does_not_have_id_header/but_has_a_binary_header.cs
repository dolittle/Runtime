// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_HeaderRequestIdentifier.when_getting_request_id_for.and_request_does_not_have_id_header;

public class but_has_a_binary_header : given.an_identifier_and_a_call_context
{
    Establish context = () =>
    {
        call_context.RequestHeaders.Add("X-Request-ID-bin", Encoding.UTF8.GetBytes("M589apGPsYYmvXQpWk3m"));
    };

    static RequestId result;
    Because of = () => result = identifier.GetRequestIdFor(call_context);

    It should_return_a_non_empty_request_id = () => result.Value.Length.Should().BeGreaterThan(0);
    It should_not_have_the_content_of_the_binary_header = () => result.Value.ShouldNotEqual("M589apGPsYYmvXQpWk3m");
}