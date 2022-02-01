// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// The exception that gets thrown when attempting to convert a property of a field that does not contain the property.
/// </summary>
public class DocumentDoesNotHaveProperty : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentDoesNotHaveProperty"/> class.
    /// </summary>
    /// <param name="document">The <see cref="BsonDocument"/> that is missing the property.</param>
    /// <param name="property">The property to be converted.</param>
    public DocumentDoesNotHaveProperty(BsonDocument document, string property)
        : base($"Document '{document} does not have property '{property}'")
    {
    }
}
