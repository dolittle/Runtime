// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters;

/// <summary>
/// Exception that gets thrown when a required field is missing from <see cref="AbstractFilterDefinition"/> document.
/// </summary>
public class AbstractFilterDefinitionDocumentIsMissingTypeField : EventStoreConsistencyError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractFilterDefinitionDocumentIsMissingTypeField"/> class.
    /// </summary>
    /// <param name="id">The _id field.</param>
    public AbstractFilterDefinitionDocumentIsMissingTypeField(Guid id)
        : base($"The Type field was missing from the filter definition document with id: {id}", null)
    {
    }
}