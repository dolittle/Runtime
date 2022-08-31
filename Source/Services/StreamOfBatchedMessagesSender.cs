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
    where TBatch : IMessage, new()
    where TData : IMessage
{
    /// <inheritdoc />
    public async Task Send(
        uint maxBatchSize,
        IAsyncEnumerator<TData> dataEnumerator,
        Action<TBatch, TData> putInBatch,
        Func<TBatch, Task> sendBatch)
    {
        try
        {
            var hasMoreStates = await dataEnumerator.MoveNextAsync().ConfigureAwait(false);
            while (hasMoreStates)
            {
                var batchToSend = new TBatch();
                putInBatch(batchToSend, dataEnumerator.Current);
                hasMoreStates = await FillBatch(maxBatchSize, dataEnumerator, batchToSend, putInBatch).ConfigureAwait(false);
                await sendBatch(batchToSend).ConfigureAwait(false);
            }
        }
        finally
        {
            await dataEnumerator.DisposeAsync().ConfigureAwait(false);
        }
    }
    
    // /// <inheritdoc />
    // public async Task Send<TDataBatch>(
    //     uint maxBatchSize,
    //     IAsyncEnumerator<TData> dataEnumerator,
    //     Action<TBatch, TDataBatch> putInBatch,
    //     Func<TBatch, TData, TDataBatch> aggregateData,
    //     Func<TBatch, Task> sendBatch)
    //     where TDataBatch : IMessage
    // {
    //     try
    //     {
    //         var hasMoreStates = await dataEnumerator.MoveNextAsync().ConfigureAwait(false);
    //         while (hasMoreStates)
    //         {
    //             var batchToSend = new TBatch();
    //             putInBatch(batchToSend, dataEnumerator.Current);
    //             hasMoreStates = await FillBatch(maxBatchSize, dataEnumerator, batchToSend, putInBatch).ConfigureAwait(false);
    //             await sendBatch(batchToSend).ConfigureAwait(false);
    //         }
    //     }
    //     finally
    //     {
    //         await dataEnumerator.DisposeAsync().ConfigureAwait(false);
    //     }
    // }
    
    static async Task<bool> FillBatch(uint maxBatchSize, IAsyncEnumerator<TData> stream, TBatch batch, Action<TBatch, TData> putInBatch)
    {
        while (await stream.MoveNextAsync().ConfigureAwait(false))
        {
            var data = stream.Current;
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
