// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Reflection;
using Google.Protobuf.Reflection;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents the definition of a client.
/// </summary>
public class Client
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="visibility">The <see cref="EndpointVisibility"/> of the hosted <see cref="Endpoint"/>.</param>
    /// <param name="type"><see cref="Type"/> of <see cref="ClientBase">client</see>.</param>
    /// <param name="serviceDescriptor"><see cref="ServiceDescriptor"/> for the service the <see cref="Client"/> is for.</param>
    public Client(EndpointVisibility visibility, Type type, ServiceDescriptor serviceDescriptor)
    {
        ThrowIfTypeDoesNotImplementClientBase(type);

        Visibility = visibility;
        Type = type;
        ServiceDescriptor = serviceDescriptor;
    }

    /// <summary>
    /// Gets the <see cref="EndpointVisibility"/> of the hosted <see cref="Endpoint"/>.
    /// </summary>
    public EndpointVisibility Visibility { get; }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="ClientBase">client</see>.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the <see cref="ServiceDescriptor"/> for the service the <see cref="Client"/> is for.
    /// </summary>
    public ServiceDescriptor ServiceDescriptor { get; }

    void ThrowIfTypeDoesNotImplementClientBase(Type type)
    {
        if (!type.Implements(typeof(ClientBase)))
        {
            throw new TypeDoesNotImplementClientBase(type);
        }
    }
}