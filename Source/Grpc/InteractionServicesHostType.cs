/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Grpc;

namespace Dolittle.Runtime.Grpc
{
    /// <summary>
    /// Represents a <see cref="IRepresentHostType">host type</see> that is for interaction communication
    /// </summary>
    /// <remarks>
    /// Interaction is considered the channel in which a representation of processes interacting
    /// </remarks>
    public class InteractionServicesHostType : IRepresentHostType
    {
        readonly Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="InteractionServicesHostType"/>
        /// </summary>
        /// <param name="configuration"><see cref="Configuration"/> containing the <see cref="HostConfiguration"/> for the host type</param>
        public InteractionServicesHostType(Configuration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public HostType Identifier => "Interaction";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindInteractionServices);

        /// <inheritdoc/>
        public HostConfiguration Configuration => _configuration.Interaction;
    }
}