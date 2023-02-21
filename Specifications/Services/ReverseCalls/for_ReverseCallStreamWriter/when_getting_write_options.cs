// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_ReverseCallStreamWriter;

public class when_getting_write_options : given.a_wrapped_stream_writer
{
    static WriteOptions options;

    Establish context = () =>
    {
        options = new WriteOptions();
        original_writer
            .SetupGet(_ => _.WriteOptions)
            .Returns(options);
    };

    static WriteOptions result;
    Because of = () => result = wrapped_writer.WriteOptions;

    It should_get_the_options_of_the_original_stream = () => original_writer.VerifyGet(_ => _.WriteOptions);
    It should_return_the_options_from_the_original_stream = () => result.ShouldEqual(options);
}