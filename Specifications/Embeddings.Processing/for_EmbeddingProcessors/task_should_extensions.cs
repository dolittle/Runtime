// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessors
{
    public static class task_should_extensions
    {
        public static object ShouldStillBeRunning(this Task task)
        {
            task.Status.ShouldNotEqual(TaskStatus.Canceled);
            task.Status.ShouldNotEqual(TaskStatus.Faulted);
            return task.Status.ShouldNotEqual(TaskStatus.RanToCompletion);
        }
    }
}