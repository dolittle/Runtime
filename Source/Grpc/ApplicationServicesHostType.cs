/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Grpc;

namespace Dolittle.Runtime.Grpc
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
        readonly Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationServicesHostType"/>
        /// </summary>
        /// <param name="configuration"><see cref="Configuration"/> containing the <see cref="HostConfiguration"/> for the host type</param>
        public ApplicationServicesHostType(Configuration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public HostType Identifier => "Application";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindApplicationServices);

        /// <inheritdoc/>
        public HostConfiguration Configuration => _configuration.Application;
    }
}