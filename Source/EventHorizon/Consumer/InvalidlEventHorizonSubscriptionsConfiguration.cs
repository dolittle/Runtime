// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when there is an invalid <see cref="InvalidlEventHorizonSubscriptionsConfiguration" />.
    /// </summary>
    public class InvalidlEventHorizonSubscriptionsConfiguration : InvalidEventHorizonConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidlEventHorizonSubscriptionsConfiguration"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="InvalidEventHorizonConfigurationReason" />.</param>
        public InvalidlEventHorizonSubscriptionsConfiguration(InvalidEventHorizonConfigurationReason reason)
            : base(EventHorizonSubscriptionsConfiguration.ConfigurationName, reason)
        {
        }
    }
}