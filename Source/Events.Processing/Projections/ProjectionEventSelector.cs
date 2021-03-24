// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Projections;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an event selector for a projection.
    /// </summary>
    /// <param name="EventType">The event type artifact.</param>
    /// <param name="KeySelectorType">The key selector type. Determines how to map an event to a read model instance./></param>
    /// <param name="KeySelectorExpression">Optional key selector expression if the <see cref="ProjectEventKeySelectorType.Property" /> flag is set.</param>
    /// <returns></returns>
    public record ProjectionEventSelector(Artifact EventType, ProjectEventKeySelectorType KeySelectorType, string KeySelectorExpression = "");
}
