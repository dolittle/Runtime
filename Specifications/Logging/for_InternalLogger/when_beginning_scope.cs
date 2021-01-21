// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Logging.for_InternalLogger
{
    public class when_beginning_scope : given.a_logger_with_two_writers
    {
        static string message;
        static object[] arguments;
        static IDisposable result;

        Establish context = () =>
        {
            message = "Some message";
            arguments = new object[] { "some arguments", 1 };
            writer_one
                .Setup(_ => _.BeginScope(Moq.It.IsAny<string>(), Moq.It.IsAny<object[]>()))
                .Returns(Disposable.Empty);
            writer_two
                .Setup(_ => _.BeginScope(Moq.It.IsAny<string>(), Moq.It.IsAny<object[]>()))
                .Returns(Disposable.Empty);
        };

        Because of = () => result = logger.BeginScope(message, arguments);

        It should_begin_scope_in_writer_one = () =>
        {
            writer_one.Verify(_ => _.BeginScope(message, arguments), Moq.Times.Once());
            writer_one.VerifyNoOtherCalls();
        };

        It should_forward_to_writer_two_with_level_debug_without_exception = () =>
        {
            writer_two.Verify(_ => _.BeginScope(message, arguments), Moq.Times.Once());
            writer_two.VerifyNoOtherCalls();
        };

        It should_return_a_dispoable = () => result.ShouldNotBeNull();

        Cleanup cleanup = () => result.Dispose();
    }
}