// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Applications;
using Dolittle.Configuration;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents the configuration for hosts by <see cref="EventHorizonsConfiguration"/>.
    /// </summary>
    [Name("event-horizons")]
    public class EventHorizonsConfiguration :
        ReadOnlyDictionary<Microservice, Subscriptions>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="Microservice"/> with <see cref="Subscriptions"/>.</param>
        public EventHorizonsConfiguration(IDictionary<Microservice, Subscriptions> configuration)
            : base(configuration)
        {
        }
    }
}