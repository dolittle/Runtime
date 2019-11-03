/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Google.Protobuf.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Heads
{
    /// <summary>
    /// Represents the definition of a client service and its actual <see cref="ClientBase{T}"/> for it
    /// </summary>
    public class HeadServiceDefinition
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HeadServiceDefinition"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> of service</param>
        /// <param name="descriptor"><see cref="ServiceDescriptor"/> describing the service</param>
        public HeadServiceDefinition(Type type, ServiceDescriptor descriptor)
        {
            ThrowIfNotInheritingFromClientBase(type);
            Type = type;
            Descriptor = descriptor;
        }

        /// <summary>
        /// Gets the type of client - an implementor of <see cref="ClientBase{T}"/>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="ServiceDescriptor"/> describing the service
        /// </summary>
        public ServiceDescriptor Descriptor { get; }

        void ThrowIfNotInheritingFromClientBase(Type type)
        {
            if (!typeof(ClientBase).IsAssignableFrom(type)) throw new MustInheritFromClientBase(type);
        }
    }
}