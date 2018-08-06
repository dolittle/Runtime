/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents a null implementation of <see cref="IEventSequenceNumbers"/>
    /// </summary>
    public class NullEventSequenceNumbers : IEventSequenceNumbers
    {
        /// <inheritdoc/>
        public EventSequenceNumber Next()
        {
            return 0L;
        }

        /// <inheritdoc/>
        public EventSequenceNumber NextForType(Artifact identifier)
        {
            return 0L;
        }
    }
}
