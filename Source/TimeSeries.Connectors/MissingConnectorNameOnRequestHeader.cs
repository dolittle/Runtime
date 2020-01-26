// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Exception that gets thrown when the 'pushconnectorname' is missing from a request header on a call.
    /// </summary>
    public class MissingConnectorNameOnRequestHeader : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConnectorNameOnRequestHeader"/> class.
        /// </summary>
        public MissingConnectorNameOnRequestHeader()
            : base("The request header requires the 'pushconnectorname' to be set to the valid name of the connector")
        {
        }
    }
}