/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Services;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents a <see cref="IRepresentServiceType">host type</see> that is for application communication
    /// </summary>
    /// <remarks>
    /// Application is considered the channel in which a representation of the application is talking - 
    /// typically and SDK
    /// </remarks>
    public class RuntimeServiceType : IRepresentServiceType
    {
        /// <inheritdoc/>
        public ServiceType Identifier => "Runtime";

        /// <inheritdoc/>
        public Type BindingInterface => typeof(ICanBindRuntimeServices);

        /// <inheritdoc/>
        public EndpointVisibility Visibility => EndpointVisibility.Private;
    }
}