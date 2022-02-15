// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents a <see cref="ICanProvideBindings">binding provider</see> for services and clients.
/// </summary>
public class Bindings : ICanProvideBindings
{
    readonly IEnumerable<Type> _clientTypes;
    readonly GetContainer _getContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bindings"/> class.
    /// </summary>
    /// <param name="typeFinder"><see cref="ITypeFinder"/> for finding implementations of <see cref="ClientBase"/>.</param>
    /// <param name="getContainer"><see cref="GetContainer"/> for getting the <see cref="IContainer"/>.</param>
    public Bindings(ITypeFinder typeFinder, GetContainer getContainer)
    {
        _clientTypes = typeFinder.FindMultiple<ClientBase>();
        _getContainer = getContainer;
    }

    /// <inheritdoc/>
    public void Provide(IBindingProviderBuilder builder)
    {
        _clientTypes.ForEach(clientType => builder.Bind(clientType).To(() => _getContainer().Get<IClientManager>().Get(clientType)));
    }
}
