// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when a type filter definition that does not match the previously persisted definition is being registered.
    /// </summary>
    public class TypeFilterDefinitionDoesNotMatchPersistedDefinition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilterDefinitionDoesNotMatchPersistedDefinition"/> class.
        /// </summary>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        public TypeFilterDefinitionDoesNotMatchPersistedDefinition(StreamId targetStream, StreamId sourceStream)
            : base($"Cannot register type filter definition with target stream '{targetStream}' and source stream '{sourceStream}' because it does not match the persisted filter definition.")
        {
        }
    }
}