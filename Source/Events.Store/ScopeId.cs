// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the identification of a scope.
    /// </summary>
    public class ScopeId : ConceptAs<Guid>
    {
        /// <summary>
        /// A static singleton instance to represent a "Default" <see cref="ScopeId" />.
        /// </summary>
        public static readonly ScopeId Default = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeId"/> class.
        /// </summary>
        public ScopeId() => Value = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeId"/> class.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> value.</param>
        public ScopeId(Guid id) => Value = id;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to an <see cref="ScopeId"/>.
        /// </summary>
        /// <param name="scopeId">ScopeId as <see cref="Guid"/>.</param>
        public static implicit operator ScopeId(Guid scopeId) => new ScopeId(scopeId);
    }
}
