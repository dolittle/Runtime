/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace doLittle.Events
{
    /// <summary>
    /// The exception that is thrown when an <see cref="IEvent"/> is out of sequence in an <see cref="IEvent">stream of events</see>
    /// </summary>
    public class EventOutOfSequenceException : ArgumentException
    {
    }
}
