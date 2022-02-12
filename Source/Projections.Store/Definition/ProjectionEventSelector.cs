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
    public static ProjectionEventSelector EventSourceId(ArtifactId eventType) => new(eventType, ProjectEventKeySelectorType.EventSourceId, "", "", "");

    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.PartitionId" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector PartitionId(ArtifactId eventType) => new(eventType, ProjectEventKeySelectorType.PartitionId, "", "", "");

    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.Property" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="expression">The event property expression.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector EventProperty(ArtifactId eventType, KeySelectorExpression expression) => new(eventType, ProjectEventKeySelectorType.Property, expression, "", "");
    
    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.Static" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="key">The static key.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector Static(ArtifactId eventType, ProjectionKey key) => new(eventType, ProjectEventKeySelectorType.Static, "", key, "");
    
    /// <summary>
    /// Creates a <see cref="ProjectionEventSelector" /> of type <see cref="ProjectEventKeySelectorType.Occurred" />.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="occurredFormat">The occurred format.</param>
    /// <returns>The <see cref="ProjectionEventSelector" />.</returns>
    public static ProjectionEventSelector Occurred(ArtifactId eventType, OccurredFormat occurredFormat) => new(eventType, ProjectEventKeySelectorType.EventOccurred, "", "", occurredFormat);

    /// <summary>
    /// Initializes an instance of the <see cref="ProjectionEventSelector" /> class.
    /// </summary>
    /// <param name="eventType">The event type id.</param>
    /// <param name="keySelectorType">The key selector type.</param>
    /// <param name="keySelectorExpression">The key selector expression.</param>
    /// <param name="staticKey">The static key.</param>
    /// <param name="occurredFormat">The occurred format.</param>
    public ProjectionEventSelector(ArtifactId eventType, ProjectEventKeySelectorType keySelectorType, KeySelectorExpression keySelectorExpression, ProjectionKey staticKey, OccurredFormat occurredFormat)
    {
        EventType = eventType;
        KeySelectorType = keySelectorType;
        KeySelectorExpression = keySelectorExpression;
        StaticKey = staticKey;
        OccurredFormat = occurredFormat;
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
    
    /// <summary>
    /// Gets the static <see cref="ProjectionKey"/> key.
    /// </summary>
    public ProjectionKey StaticKey { get; init; }
    
    /// <summary>
    /// Gets the <see cref="OccurredFormat"/>.
    /// </summary>
    public OccurredFormat OccurredFormat { get; init; }
}
