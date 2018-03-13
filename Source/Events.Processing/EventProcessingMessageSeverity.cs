/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// The severity of a <see cref="EventProcessingMessage"/>
    /// </summary>
    public enum EventProcessingMessageSeverity
    {
        /// <summary>
        /// Message is informational
        /// </summary>
        Information,

        /// <summary>
        /// Message is a warning
        /// </summary>
        Warning,

        /// <summary>
        /// Message is an error
        /// </summary>
        Error
    }
}