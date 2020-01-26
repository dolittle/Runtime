// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Connectors
{
    /// <summary>
    /// Exception that gets thrown when the 'tags' is missing from a request header on a call.
    /// </summary>
    public class MissingTagsOnRequestHeader : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTagsOnRequestHeader"/> class.
        /// </summary>
        public MissingTagsOnRequestHeader()
            : base("The request header requires the 'tags' to be set to comma separated list of tags available")
        {
        }
    }
}