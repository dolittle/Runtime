// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Projections.Store.Definition
{
    public record ProjectionEventSelector
    {
        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionEventSelector" /> class.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="keySelectorType">The key selector type.</param>
        public ProjectionEventSelector(Artifact eventType, ProjectEventKeySelectorType keySelectorType)
            : this(eventType, keySelectorType, "")
        { }

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionEventSelector" /> class.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="keySelectorType">The key selector type.</param>
        /// <param name="keySelectorExpression">The key selector expression.</param>
        public ProjectionEventSelector(Artifact eventType, ProjectEventKeySelectorType keySelectorType, KeySelectorExpression keySelectorExpression)
        {
            EventType = eventType;
            KeySelectorType = keySelectorType;
            KeySelectorExpression = keySelectorExpression;
        }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public Artifact EventType { get; init; }

        /// <summary>
        /// Gets the projection event key selector type.
        /// </summary>
        public ProjectEventKeySelectorType KeySelectorType { get; init; }

        /// <summary>
        /// Gets the key selector expression.
        /// </summary>
        public KeySelectorExpression KeySelectorExpression { get; init; }
    }
}
