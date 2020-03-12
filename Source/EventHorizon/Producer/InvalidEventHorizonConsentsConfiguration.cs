// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Exception that gets thrown when there is an invalid event horizon consents configuration.
    /// </summary>
    public class InvalidEventHorizonConsentsConfiguration : InvalidEventHorizonConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventHorizonConsentsConfiguration"/> class.
        /// </summary>
        /// <param name="reason">The <see cref="InvalidEventHorizonConfigurationReason" />.</param>
        public InvalidEventHorizonConsentsConfiguration(InvalidEventHorizonConfigurationReason reason)
            : base(EventHorizonConsentsConfiguration.ConfigurationName, reason)
        {
        }
    }
}