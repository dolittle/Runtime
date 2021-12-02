// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;
using System;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEventWatcher.given;

public class all_dependencies
{
    protected static StreamEventWatcher event_watcher;

    protected static CancellationTokenSource cancellation_token_source;
    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        event_watcher = new StreamEventWatcher(
            Mock.Of<ILogger<StreamEventWatcher>>());

        cancellation_token_source = new CancellationTokenSource();
        cancellation_token = cancellation_token_source.Token;
    };

    Cleanup clean = () =>
    {
        cancellation_token_source.Cancel();
        cancellation_token_source.Dispose();
    };
}