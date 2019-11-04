/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Services;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents a <see cref="IRepresentServiceType">host type</see> that is for interaction communication
    /// </summary>
    /// <remarks>
    /// Interaction is considered the channel in which a representation of processes interacting
    /// </remarks>
    public class MicroserviceServiceType : IRepresentServiceType
    {
        /// <inheritdoc/>
        public ServiceType Identifier => "Microservice";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindMicroserviceServices);

        /// <inheritdoc/>
        public EndpointVisibility Visibility => EndpointVisibility.Public;
    }
}