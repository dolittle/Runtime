// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Defines a system that can send a stream of data as batched messages.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch message.</typeparam>
/// <typeparam name="TData">The <see cref="Type"/> of the data to put in a batch.</typeparam>
public interface ISendStreamOfBatchedMessages<TBatch, TData>
    where TBatch : IMessage, new()
    where TData : IMessage
{
    /// <summary>
    /// Sends a stream of data as batched messages.
    /// </summary>
    /// <param name="maxBatchSize">The max batch size.</param>
    /// <param name="dataEnumerator">The stream of data to batch and send.</param>
    /// <param name="putInBatch">The callback for putting data in a batch.</param>
    /// <param name="sendBatch">The callback for sending a batch.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Send(uint maxBatchSize, IAsyncEnumerator<TData> dataEnumerator, Action<TBatch, TData> putInBatch, Func<TBatch, Task> sendBatch);

    Task Send<TDataToBatch>(
        uint maxBatchSize,
        IAsyncEnumerator<TDataToBatch> dataEnumerator,
        Action<TBatch, TDataToBatch> putInBatch,
        Func<TDataToBatch, TData> convertToData,
        Func<TBatch, Task> sendBatch);
}
