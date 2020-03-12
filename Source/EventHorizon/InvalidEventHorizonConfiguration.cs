// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when there is an illegal <see cref="InvalidEventHorizonConfiguration" />.
    /// </summary>
    public class InvalidEventHorizonConfiguration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventHorizonConfiguration"/> class.
        /// </summary>
        /// <param name="configurationName">The configuration file name of the event horizon configuration.</param>
        /// <param name="reason">The reason why this configuration is invalid.</param>
        public InvalidEventHorizonConfiguration(string configurationName, InvalidEventHorizonConfigurationReason reason)
            : base($"The event horizon configuration '{configurationName}' is invalid. {reason}")
        {
        }
    }
}