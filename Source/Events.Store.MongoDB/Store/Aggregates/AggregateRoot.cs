// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Represents the state of an Aggregate Root.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AggregateRoot
    {
        /// <summary>
        /// Gets or sets the id of the Event Source.
        /// </summary>
        public Guid EventSource { get; set; }

        /// <summary>
        /// Gets or sets the type of the Aggregate Root.
        /// </summary>
        public Guid AggregateType { get; set; }

        /// <summary>
        /// Gets or sets the version of the Aggregate Root.
        /// </summary>
        public uint Version { get; set; }
    }
}