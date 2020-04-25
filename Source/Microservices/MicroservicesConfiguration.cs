// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.ApplicationModel;
using Dolittle.Configuration;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents the configuration for microservices.
    /// </summary>
    [Name(ConfigurationName)]
    public class MicroservicesConfiguration :
        ReadOnlyDictionary<Microservice, MicroserviceAddress>,
        IConfigurationObject
    {
        /// <summary>
        /// The name of the <see cref="IConfigurationObject" />.
        /// </summary>
        public const string ConfigurationName = "microservices";

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroservicesConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">Dictionary for <see cref="Microservice"/> with <see cref="MicroserviceAddress"/>.</param>
        public MicroservicesConfiguration(IDictionary<Microservice, MicroserviceAddress> configuration)
            : base(configuration)
        {
        }
    }
}