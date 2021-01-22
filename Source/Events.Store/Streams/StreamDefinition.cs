// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents the definition of a Stream.
    /// </summary>
    public class StreamDefinition : Value<StreamDefinition>, IStreamDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinition"/> class.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        public StreamDefinition(IFilterDefinition filterDefinition)
        {
            FilterDefinition = filterDefinition;
        }

        /// <inheritdoc/>
        public IFilterDefinition FilterDefinition { get; }

        /// <inheritdoc/>
        public bool Public => FilterDefinition.Public;

        /// <inheritdoc/>
        public StreamId StreamId => FilterDefinition.TargetStream;

        /// <inheritdoc/>
        public bool Partitioned => FilterDefinition.Partitioned;

        /// <inheritdoc/>
        public override string ToString() => $"Stream Id: {StreamId} Partitioned: {Partitioned} Public: {Public}";
    }
}
