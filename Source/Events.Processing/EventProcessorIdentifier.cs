/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Concepts;

namespace doLittle.Events
{
    /// <summary>
    /// Represents a unqiue identifier for a <see cref="IEventProcessor"/>
    /// </summary>
    public class EventProcessorIdentifier : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventProcessorIdentifier"/>
        /// </summary>
        /// <param name="identifier"><see cref="string"/> representation</param>
        public static implicit operator EventProcessorIdentifier(string identifier)
        {
            return new EventProcessorIdentifier { Value = identifier };
        }
    }
}