// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents a system that can send a stream of data as batched messages.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch message.</typeparam>
/// <typeparam name="TData">The <see cref="Type"/> of the data to put in a batch.</typeparam>
public static class StreamOfBatchedMessagesSender<TBatch, TData>
    where TBatch : IMessage
    where TData : IMessage
{
    /// <summary>
    /// Sends a stream of data as batched messages.
    /// </summary>
    /// <param name="maxBatchSize">The max batch size.</param>
    /// <param name="dataEnumerator">The stream of data to batch and send.</param>
    /// <param name="createNewBatch">The callback for creating an empty batch.</param>
    /// <param name="putInBatch">The callback for putting data in a batch.</param>
    /// <param name="sendBatch">The callback for sending a batch.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static Task Send(
        uint maxBatchSize,
        IAsyncEnumerator<TData> dataEnumerator,
        Func<TBatch> createNewBatch,
        Action<TBatch, TData> putInBatch,
        Func<TBatch, Task> sendBatch)
        => Send(maxBatchSize, dataEnumerator, createNewBatch, putInBatch, _ => _, sendBatch);
    
    /// <summary>
    /// Sends a stream of data as batched messages.
    /// </summary>
    /// <param name="maxBatchSize">The max batch size.</param>
    /// <param name="dataEnumerator">The stream of data to batch and send.</param>
    /// <param name="createNewBatch">The callback for creating an empty batch.</param>
    /// <param name="putInBatch">The callback for putting data in a batch.</param>
    /// <param name="convertToData">The callback for converting <typeparamref name="TDataToBatch"/> to <typeparamref name="TData"/>.</param>
    /// <param name="sendBatch">The callback for sending a batch.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task Send<TDataToBatch>(
        uint maxBatchSize,
        IAsyncEnumerator<TDataToBatch> dataEnumerator,
        Func<TBatch> createNewBatch,
        Action<TBatch, TData> putInBatch,
        Func<TDataToBatch, TData> convertToData,
        Func<TBatch, Task> sendBatch)
    {
        try
        {
            var hasMoreData = await dataEnumerator.MoveNextAsync().ConfigureAwait(false);
            if (!hasMoreData)
            {
                await sendBatch(createNewBatch()).ConfigureAwait(false);
            }
            while (hasMoreData)
            {
                var batchToSend = createNewBatch();
                putInBatch(batchToSend, convertToData(dataEnumerator.Current));
                hasMoreData = await FillBatch(maxBatchSize, dataEnumerator, batchToSend, putInBatch, convertToData).ConfigureAwait(false);
                await sendBatch(batchToSend).ConfigureAwait(false);
            }
        }
        finally
        {
            await dataEnumerator.DisposeAsync().ConfigureAwait(false);
        }
    }

    static async Task<bool> FillBatch<TDataToBatch>(uint maxBatchSize, IAsyncEnumerator<TDataToBatch> stream, TBatch batch, Action<TBatch, TData> putInBatch, Func<TDataToBatch, TData> convertToData)
    {
        while (await stream.MoveNextAsync().ConfigureAwait(false))
        {
            var data = convertToData(stream.Current);
            
            if (BatchWouldBeTooLarge(maxBatchSize, batch, data))
            {
                return true;
            }
            putInBatch(batch, data);
        }

        return false;
    }

    static bool BatchWouldBeTooLarge(uint maxBatchSize, TBatch batch, TData data)
        => batch.CalculateSize() + data.CalculateSize() > maxBatchSize;
}
