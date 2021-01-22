// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Scheduling
{
    /// <summary>
    /// Defines an abstraction for scheduling work.
    /// </summary>
    /// <remarks>
    /// One of the things this abstraction helps with is to ensure compatibility with systems that
    /// aren't capable of running async/threaded operations. So even though C# has its TPL and
    /// async/await - these are working under the assumption that there is this capability. With
    /// this abstraction we can chose what implementation to use that is compatible with the
    /// underlying system.
    /// </remarks>
    public interface IScheduler
    {
        /// <summary>
        /// Perform an action.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        void Perform(Action action);

        /// <summary>
        /// Perform an action for each element in a collection.
        /// </summary>
        /// <param name="collection"><see cref="IEnumerable{T}"/> to perform for.</param>
        /// <param name="action">Action to perform - given the element.</param>
        /// <typeparam name="T">Type of element.</typeparam>
        void PerformForEach<T>(IEnumerable<T> collection, Action<T> action);
    }
}