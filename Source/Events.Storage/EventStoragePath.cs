/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Storage
{
    /// <summary>
    /// Represents the path within the <see cref="IEventStore"/>
    /// </summary>
    /// <remarks>
    /// A valid path includes alpha-numeric and / as separator.
    /// 
    /// e.g. Dolittle/Sentry/53928a1d-12b3-451d-afaa-b22f4f0cc410
    /// 
    /// The Guid in the path would typically then be the <see cref="EventSourceId"/>
    /// </remarks>
    public class EventStoragePath : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventStoragePath"/>
        /// </summary>
        /// <param name="path"><see cref="string"/> to convert from</param>
        public static implicit operator EventStoragePath(string path) => new EventStoragePath { Value = path };
    }
}