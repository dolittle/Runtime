/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Events
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