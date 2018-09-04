/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a unqiue identifier for a <see cref="IEventProcessor"/>
    /// </summary>
    public class EventProcessorId : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventProcessorId"/>
        /// </summary>
        /// <param name="identifier"><see cref="string"/> representation</param>
        public static implicit operator EventProcessorId(string identifier)
        {
            return new EventProcessorId { Value = identifier };
        }
    }
}