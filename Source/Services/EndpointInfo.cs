// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the information about a specific <see cref="Endpoint"/>.
    /// </summary>
    public class EndpointInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInfo"/> class.
        /// </summary>
        /// <param name="visibility"><see cref="EndpointVisibility">Type</see> of endpoint.</param>
        /// <param name="configuration"><see cref="EndpointConfiguration">Configuration</see> for the endoint.</param>
        public EndpointInfo(EndpointVisibility visibility, EndpointConfiguration configuration)
        {
            Visibility = visibility;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the visibility of the <see cref="Endpoint"/>.
        /// </summary>
        public EndpointVisibility Visibility { get; }

        /// <summary>
        /// Gets the actual <see cref="EndpointConfiguration">configuration</see> for the <see cref="Endpoint"/>.
        /// </summary>
        public EndpointConfiguration Configuration { get; }
    }
}