// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Projections.Store.State
{
    /// <summary>
    /// Represents a state from a projection.
    /// </summary>
    /// <param name="Value">The state.</param>
    /// <typeparam name="string">The type of the concept.</typeparam>
    public record ProjectionState(string Value) : ConceptAs<string>(Value);
}
