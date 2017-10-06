/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Applications;

namespace doLittle.Events
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
        public EventSequenceNumber NextForType(IApplicationResourceIdentifier identifier)
        {
            return 0L;
        }
    }
}
