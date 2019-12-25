// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A unique identifier for a <see cref="CommittedEventStream" />.
    /// </summary>
    public class CommitId : ConceptAs<Guid>
    {
        /// <summary>
        /// A static readonly instance representing an Empty <see cref="CommitId" />. Initialized with Guid.Empty.
        /// </summary>
        public static readonly CommitId Empty = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitId"/> class.
        /// </summary>
        public CommitId()
        {
            Value = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitId"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>.</param>
        public CommitId(Guid id)
        {
            Value = id;
        }

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="CommitId"/>.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> to convert from.</param>
        public static implicit operator CommitId(Guid value) => new CommitId(value);

        /// <summary>
        /// Creates a new instance of <see cref="CommitId" /> with a generated unique id.
        /// </summary>
        /// <returns>A new unqiue <see cref="CommitId"/>.</returns>
        public static CommitId New()
        {
            return new CommitId(Guid.NewGuid());
        }
    }
}