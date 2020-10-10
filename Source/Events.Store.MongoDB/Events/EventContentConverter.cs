// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventContentConverter"/>.
    /// </summary>
    public class EventContentConverter : IEventContentConverter
    {
        static readonly JsonWriterSettings ToJsonSettings = new JsonWriterSettings
        {
            OutputMode = JsonOutputMode.Strict,
            Indent = false,
        };

        /// <inheritdoc/>
        public BsonDocument ToBSON(string json)
            => BsonDocument.Parse(json);

        /// <inheritdoc/>
        public string ToJSON(BsonDocument bson)
            => bson.ToJson(ToJsonSettings);
    }
}
