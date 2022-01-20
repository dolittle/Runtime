// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Projections.Store.Definition;

public record ProjectionEventSelector
{
    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.EventSourceId" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector EventSourceId(ArtifactId eventType) => new(eventType, ProjectEventKeySelectorType.EventSourceId, "");

    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.PartitionId" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector PartitionId(ArtifactId eventType) => new(eventType, ProjectEventKeySelectorType.PartitionId, "");

    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.Property" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="expression">The event property expression.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector EventProperty(ArtifactId eventType, KeySelectorExpression expression) => new(eventType, ProjectEventKeySelectorType.Property, expression);

    /// <summary>
    /// Initializes an instance of the <see cref="ProjectionEventSelector" /> class.
    /// </summary>
    /// <param name="eventType">The event type id.</param>
    /// <param name="keySelectorType">The key selector type.</param>
    /// <param name="keySelectorExpression">The key selector expression.</param>
    public ProjectionEventSelector(ArtifactId eventType, ProjectEventKeySelectorType keySelectorType, KeySelectorExpression keySelectorExpression)
    {
        EventType = eventType;
        KeySelectorType = keySelectorType;
        KeySelectorExpression = keySelectorExpression;
    }

    /// <summary>
    /// Gets the event type.
    /// </summary>
    public ArtifactId EventType { get; init; }

    /// <summary>
    /// Gets the projection event key selector type.
    /// </summary>
    public ProjectEventKeySelectorType KeySelectorType { get; init; }

    /// <summary>
    /// Gets the key selector expression.
    /// </summary>
    public KeySelectorExpression KeySelectorExpression { get; init; }
}