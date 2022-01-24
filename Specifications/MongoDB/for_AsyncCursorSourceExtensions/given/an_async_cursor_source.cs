// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using MongoDB.Driver;
using Moq;

namespace Dolittle.Runtime.MongoDB.for_AsyncCursorSourceExtensions.given;

public class an_async_cursor_source
{
    protected static IAsyncCursorSource<T> CreateAsyncCursorSourceThatYields<T>(Action disposeCalled, params T[][] batches)
    {
        var mock = new Mock<IAsyncCursorSource<T>>();
        mock.Setup(_ => _.ToCursorAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MockAsyncCursor<T>(batches, disposeCalled));
        return mock.Object;
    }
}