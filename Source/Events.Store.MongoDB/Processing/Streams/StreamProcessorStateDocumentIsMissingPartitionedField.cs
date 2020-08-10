// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        /// <param name="id">The _id field.</param>
        public StreamProcessorStateDocumentIsMissingPartitionedField(ObjectId id)
            : base($"StreamProcessorStateDiscriminatorConvention couldn't find a \"Partitioned\" field from document with id: {id}, causing AbstractStreamProcessorState collection deserialization to fail.", null)
        {
        }
    }
}
