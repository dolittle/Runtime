/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Events
{
    /// <summary>
    /// Represents the status of an <see cref="IEventProcessor"/>
    /// </summary>
    public enum EventProcessorStatus
    {
        /// <summary>
        /// The <see cref="IEventProcessor"/> is online
        /// </summary>
        Online,

        /// <summary>
        /// The <see cref="IEventProcessor"/> is streaming events
        /// </summary>
        Streaming
    }
}