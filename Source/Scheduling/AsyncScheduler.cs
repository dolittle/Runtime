// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Scheduling
{
    /// <summary>
    /// Represents an implementation of <see cref="IScheduler"/> for scheduling asynchronously.
    /// </summary>
    public class AsyncScheduler : IScheduler
    {
        /// <inheritdoc/>
        public void Perform(Action action)
        {
            Task.Run(action);
        }

        /// <inheritdoc/>
        public void PerformForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            Parallel.ForEach(collection, action);
        }
    }
}