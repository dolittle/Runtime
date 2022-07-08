// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.MongoDB.for_AsyncCursorSourceExtensions.given;

public class MockAsyncCursor<T> : IAsyncCursor<T>
{
    readonly T[][] _batches;
    readonly Action _disposeCalled;
    int _currentIndex = -1;

    public MockAsyncCursor(T[][] batches, Action disposeCalled)
    {
        _batches = batches;
        _disposeCalled = disposeCalled;
    }

    public bool MoveNext(CancellationToken cancellationToken = default)
    {
        _currentIndex += 1;

        if (_currentIndex > _batches.Length)
        {
            throw new Exception("Already iterated through the whole cursor");
        }

        return _currentIndex < _batches.Length;
    }

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(MoveNext(cancellationToken));

    public IEnumerable<T> Current
    {
        get
        {
            if (_currentIndex >= _batches.Length)
            {
                throw new Exception("Cannot get current after end of cursor");
            }

            return _batches[_currentIndex];
        }
    }

    public void Dispose() => _disposeCalled();
}