// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Defines a system that can convert between BSON and JSON representations of event content.
/// </summary>
public interface IEventContentConverter
{
    /// <summary>
    /// Converts a JSON <see cref="string"/> to a <see cref="BsonDocument"/>.
    /// </summary>
    /// <param name="json">The JSON content to convert.</param>
    /// <returns>The converted <see cref="BsonDocument"/>.</returns>
    BsonDocument ToBson(string json);

    /// <summary>
    /// Convertsa a <see cref="BsonDocument"/> to a JSON <see cref="string"/>.
    /// </summary>
    /// <param name="bson">The BSON content to convert.</param>
    /// <returns>The converted JSON <see cref="string"/>.</returns>
    string ToJson(BsonDocument bson);
}