// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{
    /// <summary>
    /// Exception that gets thrown when a field is missing from <see cref="AbstractFilterDefinition"/> document.
    /// </summary>
    public class AbstractFilterDefinitionDocumentIsMissingAField : EventStoreConsistencyError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFilterDefinitionDocumentIsMissingAField"/> class.
        /// </summary>
        /// <param name="id">The _id field.</param>
        /// <param name="field">The missing field.</param>
        public AbstractFilterDefinitionDocumentIsMissingAField(Guid id, string field)
            : base($"The {field} was missing from the document with id: {id}", null)
        {
        }
    }
}
