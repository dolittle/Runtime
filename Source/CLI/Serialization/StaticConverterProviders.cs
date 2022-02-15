// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Serialization.Json;

namespace Dolittle.Runtime.CLI.Serialization;

/// <summary>
/// Represents an implementation of <see cref="InstancesOf{T}"/> of <see cref="ICanProvideConverters"/> that consists of a static set of providers.
/// </summary>
public class StaticConverterProviders : IEnumerable<ICanProvideConverters>
{
    readonly ICanProvideConverters[] _providers;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticConverterProviders"/> class.
    /// </summary>
    /// <param name="providers">The providers that will be presented.</param>
    public StaticConverterProviders(params ICanProvideConverters[] providers)
    {
        _providers = providers;
    }

    /// <inheritdoc />
    public IEnumerator<ICanProvideConverters> GetEnumerator()
        => _providers.Cast<ICanProvideConverters>().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => _providers.GetEnumerator();
}
