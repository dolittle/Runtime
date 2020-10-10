// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventContentConverter"/>.
    /// </summary>
    public class EventContentConverter : IEventContentConverter
    {
        /// <inheritdoc/>
        public BsonDocument ToBSON(string json)
            => BsonDocument.Parse(json);

        /// <inheritdoc/>
        public string ToJSON(BsonDocument bson)
            => bson.ToString();
    }
}
