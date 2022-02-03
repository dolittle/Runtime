// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// The exception that gets thrown when attempting to convert a property on a field that is not a document.
/// </summary>
public class ValueIsNotDocument : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueIsNotDocument"/> class.
    /// </summary>
    /// <param name="type">The <see cref="BsonType"/> of the field.</param>
    /// <param name="property">The property to be converted.</param>
    public ValueIsNotDocument(BsonType type, string property)
        : base($"Cannot convert property '{property}'. Expected a document, found ${type}")
    {
    }
}
