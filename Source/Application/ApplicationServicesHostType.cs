/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Hosting;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents a <see cref="IRepresentHostType">host type</see> that is for application communication
    /// </summary>
    /// <remarks>
    /// Application is considered the channel in which a representation of the application is talking - 
    /// typically and SDK
    /// </remarks>
    public class ApplicationServicesHostType : IRepresentHostType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationServicesHostType"/>
        /// </summary>
        /// <param name="configuration"><see cref="HostsConfiguration"/> containing the <see cref="HostConfiguration"/> for the host type</param>
        public ApplicationServicesHostType(HostsConfiguration configuration)
        {
            Configuration = configuration.ContainsKey(Identifier)?configuration[Identifier]:new HostConfiguration(50053);
        }

        /// <inheritdoc/>
        public HostType Identifier => "Application";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindApplicationServices);

        /// <inheritdoc/>
        public HostConfiguration Configuration { get; }
    }
}