// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a common way to validate two filtered streams.
    /// </summary>
    public static class FilteredStreamsValidator
    {
        /// <summary>
        /// Validates the artifacts of two filtered streams.
        /// </summary>
        /// <param name="sourceStream">The source <see cref="StreamId" />.</param>
        /// <param name="targetStream">The target <see cref="StreamId" />.</param>
        /// <param name="oldArtifacts">The <see cref="IEnumerable{T}" /> of <see cref="Artifact" /> from the old filtered stream.</param>
        /// <param name="newArtifacts">The <see cref="IEnumerable{T}" /> of <see cref="Artifact" /> from the new filtered stream.</param>
        public static void ValidateArtifactsFromFilteredStreams(StreamId sourceStream, StreamId targetStream, IEnumerable<Artifact> oldArtifacts, IEnumerable<Artifact> newArtifacts)
        {
            if (oldArtifacts.LongCount() != newArtifacts.LongCount() || !oldArtifacts.Select(_ => _.Id).All(newArtifacts.Select(_ => _.Id).Contains)) throw new IllegalFilterTransformation(targetStream, sourceStream);
        }
    }
}