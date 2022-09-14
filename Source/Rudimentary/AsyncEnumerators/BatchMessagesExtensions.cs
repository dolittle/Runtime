// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Rudimentary.AsyncEnumerators;

/// <summary>
/// Represents extension methods to <see cref="IAsyncEnumerable{T}"/> of type <see cref="IMessage"/> for creating batches from a stream of messages.
/// </summary>
public static class BatchMessagesExtensions
{
    /// <summary>
    /// Projects an async-enumerable of messages into an async-enumerable of message batches with a given max size.
    /// </summary>
    /// <param name="messages">The <see cref="IAsyncEnumerable{T}"/> of messages to batch.</param>
    /// <param name="maxBatchSize">The max batch size (in bytes) to create.</param>
    /// <param name="cancellationToken">The optional cancellation token to be used for cancelling the batching at any time.</param>
    /// <typeparam name="T">The type of the <see cref="IMessage"/> to batch.</typeparam>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="IEnumerable{T}"/> message batches.</returns>
    public static async IAsyncEnumerable<IEnumerable<T>> CreateMessageBatchesOfSize<T>(this IAsyncEnumerable<T> messages, uint maxBatchSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where T : IMessage
    {
        var currentBatch = new List<T>();
        var currentBatchSize = 0;

        await foreach (var message in messages.WithCancellation(cancellationToken))
        {
            var messageSize = message.CalculateSize();

            // There is room for more in the batch
            if (currentBatchSize + messageSize <= maxBatchSize)
            {
                currentBatch.Add(message);
                currentBatchSize += messageSize;
                continue;
            }

            // There isn't room in the batch. If we have accumulated something - send it
            // If the message is too big in itself, it will be sent on the next enumeration alone
            if (currentBatch.Count > 0)
            {
                yield return currentBatch;
            }

            // Next batch starts with the current message
            currentBatch = new List<T> {message};
            currentBatchSize = messageSize;
        }
        
        // Send remaining messages, or an empty batch if nothing was found
        yield return currentBatch;
    }

    /// <summary>
    /// Reduces an async-enumerable of messages into an async-enumerable of messages that have been reduced to a given max size.
    /// </summary>
    /// <param name="messages">The <see cref="IAsyncEnumerable{T}"/> of messages to reduce to batches.</param>
    /// <param name="reducer">The reducer to apply to the batches.</param>
    /// <param name="maxBatchSize">The max batch size (in bytes) to create.</param>
    /// <param name="cancellationToken">The optional cancellation token to be used for cancelling the reduce batching at any time.</param>
    /// <typeparam name="T">The type of the <see cref="IMessage"/> to batch.</typeparam>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of reduced batch message.</returns>
    public static async IAsyncEnumerable<T> BatchReduceMessagesOfSize<T>(this IAsyncEnumerable<T> messages, Func<T, T, T> reducer, uint maxBatchSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where T : IMessage
    {
        // Create batches of appropriate size
        var batchedMessageLists = CreateMessageBatchesOfSize(messages, maxBatchSize, cancellationToken).Select(_ => _.ToList());
        await foreach (var batchMessages in batchedMessageLists.WithCancellation(cancellationToken))
        {
            if (!batchMessages.Any())
            {
                continue;
            }
            
            // If there are any messages
            // Reduce them using the reducer
            yield return batchMessages.Aggregate(reducer);
        }
    }
}
