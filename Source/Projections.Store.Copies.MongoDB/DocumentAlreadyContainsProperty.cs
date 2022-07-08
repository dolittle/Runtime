// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// The exception that gets thrown when attempting to rename a property in a read model and the property name is already used.
/// </summary>
public class DocumentAlreadyContainsProperty : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentAlreadyContainsProperty"/> class.
    /// </summary>
    /// <param name="document">The <see cref="BsonDocument"/> that already contains the property.</param>
    /// <param name="property">The property name that is already used.</param>
    public DocumentAlreadyContainsProperty(BsonDocument document, string property)
        : base($"Document '{document} already contains property '{property}'")
    {
    }
}
