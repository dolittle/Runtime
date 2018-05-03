/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents a template for representing dynamic <see cref="EventStoragePath">event storage paths</see>
    /// </summary>
    public class EventStoragePathTemplate
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventStoragePathTemplate"/>
        /// </summary>
        /// <param name="string">String representation of the source</param>
        /// <param name="function">Function for the template resolving</param>
        public EventStoragePathTemplate(string @string, Func<EventStorageContext, string> function)
        {
            String = @string;
            Function = function;
        }

        /// <summary>
        /// Gets the string representing the <see cref="EventStoragePathTemplate"/>
        /// </summary>
        public string String { get; }

        /// <summary>
        /// Gets the function that is able to transform the string with a given context
        /// </summary>
        public Func<EventStorageContext, string> Function { get; }
    }
}