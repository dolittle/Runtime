// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Exception that gets thrown when trying to use an unsupported type in  <see cref="StreamProcessorStateDiscriminatorConvention"/>.
    /// </summary>
    public class StreamProcessorStateDocumentIsMissingPartitionedField : EventStoreConsistencyError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStateDocumentIsMissingPartitionedField"/> class.
        /// </summary>
        /// <param name="type">The given state.</param>
        /// <param name="id">The _id field.</param>
        public StreamProcessorStateDocumentIsMissingPartitionedField(Type type, ObjectId id)
            : base($"Type: {type} with id: {id} didn't contain a \"Partitioned\" field.", null)
        {
        }
    }
}
