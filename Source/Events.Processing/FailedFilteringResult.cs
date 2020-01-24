// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterResult" /> where filtering failed.
    /// </summary>
    public class FailedFilteringResult : IFilterResult
    {
        /// <inheritdoc />
        public bool Succeeded => false;

        /// <inheritdoc />
        public bool Retry => false;

        /// <inheritdoc />
        public bool IsIncluded => false;

        /// <inheritdoc />
        public int Partition => 0;
    }
}