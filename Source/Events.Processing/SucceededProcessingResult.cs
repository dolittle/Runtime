// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProcessingResult" /> where processing succeeded.
    /// </summary>
    public class SucceededProcessingResult : IProcessingResult
    {
        /// <inheritdoc />
        public bool Succeeded => true;

        /// <inheritdoc />
        public bool Retry => false;
    }
}