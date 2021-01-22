// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can fetch <see cref="ArtifactId">event types</see> from <see cref="StreamId">stream</see>.
    /// </summary>
    public interface ICanFetchEventTypesFromStream
    {
        /// <summary>
        /// Fetch the unique <see cref="Artifact">event types</see> in an inclusive range in a <see cref="StreamId" />.
        /// </summary>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IEnumerable{Artifact}" /> event types.</returns>
        Task<IEnumerable<Artifact>> FetchInRange(StreamPositionRange range, CancellationToken cancellationToken);
    }
}
