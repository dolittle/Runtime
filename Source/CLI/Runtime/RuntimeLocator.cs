// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Services.Configuration;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// Represents an implementation of <see cref="ICanLocateRuntimes"/>.
/// </summary>
public class RuntimeLocator : ICanLocateRuntimes
{
    readonly IEnumerable<ICanDiscoverRuntimeAddresses> _addressProviders;
    const string DefaultRuntimeHost = "localhost";

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeLocator"/> class.
    /// </summary>
    /// <param name="addressProviders">The implementations of Runtime address providers to use to discover Runtimes.</param>
    public RuntimeLocator(IEnumerable<ICanDiscoverRuntimeAddresses> addressProviders)
    {
        _addressProviders = addressProviders;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NamedRuntimeAddress>> GetAvailableRuntimeAddresses(MicroserviceAddress argument = null)
    {
        if (argument == null)
        {
            var results = await Task.WhenAll(_addressProviders.Select(_ => _.Discover()));
            return results.SelectMany(_ => _);
        }
            
        var address = new NamedRuntimeAddress(
            MicroserviceName.NotSet,
            string.IsNullOrWhiteSpace(argument.Host) ? DefaultRuntimeHost : argument.Host,
            argument.Port == 0 ? new EndpointsConfiguration().Management.Port : argument.Port);
                
        return [address];
    }
}
