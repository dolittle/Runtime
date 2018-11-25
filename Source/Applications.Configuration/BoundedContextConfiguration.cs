/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Configuration;
using Dolittle.ResourceTypes;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Represents the definition of a <see cref="BoundedContext"/> for configuration
    /// </summary>
    [Name("bounded-context")]
    public class BoundedContextConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BoundedContextConfiguration"/>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="boundedContext"></param>
        /// <param name="boundedContextName"></param>
        /// <param name="core"></param>
        /// <param name="interaction"></param>
        /// <param name="resources"></param>
        public BoundedContextConfiguration(
            Application application,
            BoundedContext boundedContext,
            BoundedContextName boundedContextName,
            CoreConfiguration core,
            IEnumerable<InteractionLayerConfiguration> interaction,
            IDictionary<ResourceType, ResourceTypeImplementationConfiguration> resources)
        {
            Application = application;
            BoundedContext = boundedContext;
            BoundedContextName = boundedContextName;
            Core = core;
            Interaction = interaction;
            Resources = resources;
        }

        /// <summary>
        /// Gets or sets the <see cref="Application"/>
        /// </summary>
        public BoundedContextConfiguration(Application application, BoundedContextName boundedContextName, CoreConfiguration core) 
        {
            this.Application = application;
                this.BoundedContextName = boundedContextName;
                this.Core = core;
               
        }
                public Application Application { get; }

        /// <summary>
        /// Gets or sets the <see cref="BoundedContext"/>
        /// </summary>
        public BoundedContext BoundedContext {  get; }

        /// <summary>
        /// Gets or sets the <see cref="BoundedContextName"/> 
        /// </summary>
        public BoundedContextName BoundedContextName { get; }

        /// <summary>
        /// Gets or sets the <see cref="CoreConfiguration"/>
        /// </summary>
        public CoreConfiguration Core { get; }

        /// <summary>
        /// Gets or sets the <see cref="InteractionLayerConfiguration"/> list
        /// </summary>
        public IEnumerable<InteractionLayerConfiguration> Interaction { get; } = new InteractionLayerConfiguration[0];

        /// <summary>
        /// Gets or sets the Resource configurations
        /// </summary>
        public IDictionary<ResourceType, ResourceTypeImplementationConfiguration> Resources { get; } = new Dictionary<ResourceType, ResourceTypeImplementationConfiguration>();
    }
}