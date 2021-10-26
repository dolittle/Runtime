// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a name alias of an Event Type.
    /// </summary>
    public record EventTypeAlias(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Gets the <see cref="EventTypeAlias"/> to use when none is provided by the Client.
        /// </summary>
        public static EventTypeAlias NotSet => "No alias";
        
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventTypeAlias"/>.
        /// </summary>
        /// <param name="alias"><see cref="string"/> representation.</param>
        public static implicit operator EventTypeAlias(string alias) => new(alias);
    }
}
