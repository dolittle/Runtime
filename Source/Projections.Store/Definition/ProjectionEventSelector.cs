// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Projections.Store.Definition
{
    /// <summary>
    /// Represents the projection event selector for a specific event type.
    /// </summary>
    /// <param name="EventType">The event type.</param>
    /// <param name="KeySelectorType">The <see cref="ProjectionEventKeySelectorType" />.</param>
    /// <param name="KeySelectorExpression">The key selector expression.</param>
    /// <returns></returns>
    public record ProjectionEventSelector(
        Artifact EventType,
        ProjectEventKeySelectorType KeySelectorType,
        string KeySelectorExpression = "");
}
