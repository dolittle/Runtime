// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.Configuration;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents the configuration for hosts by <see cref="EndpointVisibility"/>.
    /// </summary>
    [Name("endpoints")]
    public class EndpointsConfiguration :
        ReadOnlyDictionary<EndpointVisibility, EndpointConfiguration>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointsConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="EndpointVisibility"/> with <see cref="EndpointConfiguration"/>.</param>
        public EndpointsConfiguration(IDictionary<EndpointVisibility, EndpointConfiguration> configuration)
            : base(configuration)
        {
        }
    }
}