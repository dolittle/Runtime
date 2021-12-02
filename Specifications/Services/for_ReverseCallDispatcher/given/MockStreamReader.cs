// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.Runtime.Services.for_ReverseCallDispatcher.given;

public class MockStreamReader : IAsyncStreamReader<MyClientMessage>
{
    TaskCompletionSource<bool> _nextPendingRead = new();

    public MyClientMessage Current { get; private set; }

    public Task<bool> MoveNext(CancellationToken cancellationToken)
        => _nextPendingRead.Task;

    public void ReceiveMessage(MyClientMessage messageToReceive)
    {
        var pendingRead = _nextPendingRead;
        _nextPendingRead = new TaskCompletionSource<bool>();
        Current = messageToReceive;
        pendingRead.SetResult(true);
    }
}