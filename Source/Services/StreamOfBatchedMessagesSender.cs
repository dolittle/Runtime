// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="ISendStreamOfBatchedMessages{TBatch,TData}"/>.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch message.</typeparam>
/// <typeparam name="TData">The <see cref="Type"/> of the data to put in a batch.</typeparam>
public class StreamOfBatchedMessagesSender<TBatch, TData> : ISendStreamOfBatchedMessages<TBatch, TData>
    where TBatch : IMessage
    where TData : IMessage
{
    /// <inheritdoc />
    public Task Send(
        uint maxBatchSize,
        IAsyncEnumerator<TData> dataEnumerator,
        Func<TBatch> createNewBatch,
        Action<TBatch, TData> putInBatch,
        Func<TBatch, Task> sendBatch)
        => Send(maxBatchSize, dataEnumerator, createNewBatch, putInBatch, _ => _, sendBatch);
    
    /// <inheritdoc />
    public async Task Send<TDataToBatch>(
        uint maxBatchSize,
        IAsyncEnumerator<TDataToBatch> dataEnumerator,
        Func<TBatch> createNewBatch,
        Action<TBatch, TDataToBatch> putInBatch,
        Func<TDataToBatch, TData> convertToData,
        Func<TBatch, Task> sendBatch)
    {
        try
        {
            var hasMoreStates = await dataEnumerator.MoveNextAsync().ConfigureAwait(false);
            var batchToSend = createNewBatch();
            if (!hasMoreStates)
            {
                await sendBatch(batchToSend).ConfigureAwait(false);
                return;
            }
            while (hasMoreStates)
            {
                putInBatch(batchToSend, dataEnumerator.Current);
                hasMoreStates = await FillBatch(maxBatchSize, dataEnumerator, batchToSend, putInBatch, convertToData).ConfigureAwait(false);
                await sendBatch(batchToSend).ConfigureAwait(false);
            }
        }
        finally
        {
            await dataEnumerator.DisposeAsync().ConfigureAwait(false);
        }
    }

    static async Task<bool> FillBatch<TDataToBatch>(uint maxBatchSize, IAsyncEnumerator<TDataToBatch> stream, TBatch batch, Action<TBatch, TDataToBatch> putInBatch, Func<TDataToBatch, TData> convertToData)
    {
        while (await stream.MoveNextAsync().ConfigureAwait(false))
        {
            var data = stream.Current;
            if (BatchWouldBeTooLarge(maxBatchSize, batch, convertToData(data)))
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
