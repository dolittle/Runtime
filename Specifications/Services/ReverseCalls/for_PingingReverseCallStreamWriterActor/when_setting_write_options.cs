// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_PingingReverseCallStreamWriterActor;

public class when_setting_write_options : given.a_wrapped_stream_writer
{
    static WriteOptions options;

    Establish context = () => options = new WriteOptions();

    Because of = () => writer.WriteOptions = options;

    It should_set_the_options_of_the_original_stream = () => original_writer.VerifySet(_ => _.WriteOptions = options);
}