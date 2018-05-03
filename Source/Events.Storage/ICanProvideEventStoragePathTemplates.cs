/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Defines a system that can provide string representations of <see cref="EventStoragePathTemplate"/>
    /// </summary>
    public interface ICanProvideEventStoragePathTemplates
    {
        /// <summary>
        /// Provide string representations of <see cref="EventStoragePathTemplate"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Provide();
    }
}