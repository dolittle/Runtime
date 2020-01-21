// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents the identification of a stream.
    /// </summary>
    public class StreamId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents a not set <see cref="StreamId"/>.
        /// </summary>
        public static StreamId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="StreamId"/>.
        /// </summary>
        /// <param name="streamId"><see cref="Guid"/> representation.</param>
        public static implicit operator StreamId(Guid streamId) => new StreamId { Value = streamId };

        /// <summary>
        /// Creates a new instance of <see cref="StreamId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="StreamId"/>.</returns>
        public static StreamId New()
        {
            return new StreamId { Value = Guid.NewGuid() };
        }
    }
}
