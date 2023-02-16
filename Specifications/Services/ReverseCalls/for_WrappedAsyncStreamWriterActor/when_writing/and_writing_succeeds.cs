// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriterActor.when_writing;

public class and_writing_succeeds : given.a_wrapped_stream_writer
{
    static a_message message;

    Establish context = () =>
    {
        message = new a_message();

        original_writer
            .Setup(_ => _.WriteAsync(message))
            .Returns(Task.CompletedTask);
    };

    Because of = () => wrapped_writer.WriteAsync(message).GetAwaiter().GetResult();

    It should_write_the_message_to_the_original_stream = () => original_writer.Verify(_ => _.WriteAsync(message));
}