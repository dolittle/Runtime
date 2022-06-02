// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Represents a batched item in flight.
/// The associated <see cref="Task"/> is completed when the <see cref="TaskCompletionSource{TResult}"/> of the <see cref="ReadyBatch{TBatchItem}"/> that it is contained in has been completed.
/// </summary>
/// <param name="Item">The batched item.</param>
/// <param name="Completed">A <see cref="Task"/> that is completed when the <see cref="ReadyBatch{TBatchItem}"/> is completed.</param>
/// <typeparam name="TItem">The <see cref="Type"/> of the item to be stored in a batch.</typeparam>
public record BatchedItem<TItem>(TItem Item, Task<Try> Completed);
