/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Grpc;

namespace Dolittle.Runtime.Grpc
{
    /// <summary>
    /// Represents a <see cref="IRepresentHostType">host type</see> that is for management communication
    /// </summary>
    /// <remarks>
    /// Management is considered the channel where tooling is connecting for management
    /// </remarks>
    public class ManagementServicesHostType : IRepresentHostType
    {
        readonly Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagementServicesHostType"/>
        /// </summary>
        /// <param name="configuration"><see cref="Configuration"/> containing the <see cref="HostConfiguration"/> for the host type</param>
        public ManagementServicesHostType(Configuration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public HostType Identifier => "Interaction";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindInteractionServices);

        /// <inheritdoc/>
        public HostConfiguration Configuration => _configuration.Application;
    }
}