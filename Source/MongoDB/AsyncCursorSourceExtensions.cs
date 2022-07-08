// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace Dolittle.Runtime.MongoDB;

/// <summary>
/// Represents extension methods for <see cref="IAsyncCursorSource{TDocument}"/>.
/// </summary>
public static class AsyncCursorSourceExtensions
{
    /// <summary>
    /// Converts the async cursor source to an async enumerable.
    /// </summary>
    /// <param name="source">The source to convert.</param>
    /// <param name="cancellationToken">The cancellation token to use to cancel the operation.</param>
    /// <typeparam name="TDocument">The type of the documents in the async cursor source.</typeparam>
    /// <returns>An <see cref="IAsyncEnumerable{TDocument}"/> to iterate over asynchronously.</returns>
    public static async IAsyncEnumerable<TDocument> ToAsyncEnumerable<TDocument>(this IAsyncCursorSource<TDocument> source, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var cursor = await source.ToCursorAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var document in cursor.Current)
                {
                    yield return document;
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
        finally
        {
            if (cursor is AsyncCursor<TDocument> asyncCursor)
            {
                await asyncCursor.CloseAsync(cancellationToken).ConfigureAwait(false);
            }
            cursor.Dispose();
        }
    }
}
