// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Exception that gets thrown when the 'pushconnectorid' is missing from a request header on a call.
    /// </summary>
    public class MissingConnectorIdentifierOnRequestHeader : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingConnectorIdentifierOnRequestHeader"/> class.
        /// </summary>
        public MissingConnectorIdentifierOnRequestHeader()
            : base("The request header requires the 'pushconnectorid' to be set to a valid GUID representing the connector")
        {
        }
    }
}