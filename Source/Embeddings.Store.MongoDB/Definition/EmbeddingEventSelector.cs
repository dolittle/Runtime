// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    public class EmbeddingEventSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="ProjectEventKeySelectorType" />.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public ProjectEventKeySelectorType EventKeySelectorType { get; set; }

        /// <summary>
        /// Gets or sets the event key selector expression.
        /// </summary>
        public string EventKeySelectorExpression { get; set; }

        /// <summary>
        /// Gets or sets the event type id that this definition filters on.
        /// </summary>
        public Guid EventType { get; set; }
    }
}
