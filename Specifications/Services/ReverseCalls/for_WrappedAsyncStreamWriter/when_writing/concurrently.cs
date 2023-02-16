// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Services.ReverseCalls.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.ReverseCalls.for_WrappedAsyncStreamWriter.when_writing;

public class concurrently : given.a_wrapped_stream_writer
{
    static a_message first_message;
    static a_message second_message;
    static a_message third_message;
    static TaskCompletionSource first_message_write;
    static TaskCompletionSource second_message_write;
    static TaskCompletionSource third_message_write;
    static int[] call_order;

    Establish context = () =>
    {
        first_message = new a_message();
        second_message = new a_message();
        third_message = new a_message();

        first_message_write = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        second_message_write = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        third_message_write = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        call_order = new int[3];
        var current_call = 1;

        original_writer
            .Setup(_ => _.WriteAsync(first_message))
            .Callback(() => call_order[0] = current_call++)
            .Returns(first_message_write.Task);
        original_writer
            .Setup(_ => _.WriteAsync(second_message))
            .Callback(() => call_order[1] = current_call++)
            .Returns(second_message_write.Task);
        original_writer
            .Setup(_ => _.WriteAsync(third_message))
            .Callback(() => call_order[2] = current_call++)
            .Returns(third_message_write.Task);
    };

    static bool first_write_first_check;
    static bool second_write_first_check;
    static bool third_write_first_check;
    static bool first_write_second_check;
    static bool second_write_second_check;
    static bool third_write_second_check;
    static bool first_write_third_check;
    static bool second_write_third_check;
    static bool third_write_third_check;

    private Because of = () =>
    {
        Task first_write_status = wrapped_writer.WriteAsync(first_message);
        Task second_write_status = wrapped_writer.WriteAsync(second_message);
        Task third_write_status = wrapped_writer.WriteAsync(third_message);

        Thread.Sleep(20);
        first_write_first_check = first_write_status.IsCompleted;
        second_write_first_check = second_write_status.IsCompleted;
        third_write_first_check = third_write_status.IsCompleted;

        first_message_write.SetResult();
        third_message_write.SetResult();
        Thread.Sleep(20);
        first_write_second_check = first_write_status.IsCompleted;
        second_write_second_check = second_write_status.IsCompleted;
        third_write_second_check = third_write_status.IsCompleted;

        second_message_write.SetResult();
        Thread.Sleep(20);
        first_write_third_check = first_write_status.IsCompleted;
        second_write_third_check = second_write_status.IsCompleted;
        third_write_third_check = third_write_status.IsCompleted;
    };

    It should_not_have_completed_any_writes_after_the_first_check = () =>
    {
        first_write_first_check.ShouldBeFalse();
        second_write_first_check.ShouldBeFalse();
        third_write_first_check.ShouldBeFalse();
    };
    It should_have_completed_the_first_after_the_second_check = () =>
    {
        first_write_second_check.ShouldBeTrue();
        second_write_second_check.ShouldBeFalse();
        third_write_second_check.ShouldBeFalse();
    };
    It should_have_completed_all_writes_after_the_third_check = () =>
    {
        first_write_third_check.ShouldBeTrue();
        second_write_third_check.ShouldBeTrue();
        third_write_third_check.ShouldBeTrue();
    };

    It should_call_write_on_the_original_stream_in_order = () =>
    {
        call_order[0].ShouldEqual(1);
        call_order[1].ShouldEqual(2);
        call_order[2].ShouldEqual(3);
    };
}