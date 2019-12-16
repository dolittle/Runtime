// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// A key to identify a particular <see cref="Application" /> and <see cref="BoundedContext" /> combination.
    /// </summary>
    public class EventHorizonKey : Value<EventHorizonKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonKey"/> class.
        /// </summary>
        /// <param name="application"><see cref="Application"/> part of the key.</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> part of the key.</param>
        public EventHorizonKey(Application application, BoundedContext boundedContext)
        {
            Application = application;
            BoundedContext = boundedContext;
        }

        /// <summary>
        /// Gets the Application.
        /// </summary>
        public Application Application { get; }

        /// <summary>
        /// Gets the BoundedContext.
        /// </summary>
        public BoundedContext BoundedContext { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Application} {BoundedContext}";
        }
    }
}