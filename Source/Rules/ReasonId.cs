// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Rules
{
    /// <summary>
    /// Represents a unique identifier for a <see cref="Reason"/>. The value type of this is a <see cref="Guid"/>.
    /// </summary>
    /// <remarks>
    /// The format of the Guid has to be :
    /// 00000000-0000-0000-0000-000000000000.
    /// </remarks>
    public record ReasonId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="ReasonId"/>.
        /// </summary>
        /// <param name="value"><see cref="Guid"/> to convert from.</param>
        public static implicit operator ReasonId(Guid value) => new(value);

        /// <summary>
        /// Implicitly convert from a string to a <see cref="ReasonId"/>.
        /// </summary>
        /// <param name="value"><see cref="string"/> representation of the <see cref="ReasonId"/> which.</param>
        public static implicit operator ReasonId(string value) => Guid.Parse(value);
    }
}
