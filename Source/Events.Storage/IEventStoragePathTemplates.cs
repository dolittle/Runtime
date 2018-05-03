/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system for dealing with <see cref="EventStoragePathTemplate"/>
    /// </summary>
    public interface IEventStoragePathTemplates
    {
        /// <summary>
        /// Gets all <see cref="EventStoragePathTemplate"/> instances available
        /// </summary>
        IEnumerable<EventStoragePathTemplate> All { get; }

        /// <summary>
        /// Add a string representation of the template
        /// </summary>
        /// <param name="template">String to add</param>
        void Add(string template);
    }
}