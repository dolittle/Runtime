// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Services.Configuration;

namespace Dolittle.Runtime.CLI.Runtime;

/// <summary>
/// An implementation of <see cref="ICanDiscoverRuntimeAddresses"/> that finds Runtimes running with Docker.
/// </summary>
public class DockerRuntimeAddresses : ICanDiscoverRuntimeAddresses
{
    const string RuntimeImagePrefix = "dolittle/runtime";
        
    readonly IDockerClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRuntimeAddresses"/> class.
    /// </summary>
    /// <param name="client">The Docker client to use to list containers.</param>
    public DockerRuntimeAddresses(IDockerClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NamedRuntimeAddress>> Discover()
    {
        try
        {
            var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters());
            var runtimesWithExposedManagement =
                containers.Where(IsRuntimeContainer).Where(HasManagementPortExposed);
            return runtimesWithExposedManagement.Select(ManagementPortAddress);
        }
        catch
        {
            return Array.Empty<NamedRuntimeAddress>();
        }
    }

    static bool IsRuntimeContainer(ContainerListResponse container)
        => container.Image.StartsWith(RuntimeImagePrefix, StringComparison.InvariantCultureIgnoreCase);

    static bool HasManagementPortExposed(ContainerListResponse container)
        => container.Ports.Any(IsManagementPort);
        
    static NamedRuntimeAddress ManagementPortAddress(ContainerListResponse container)
        => new(
            GetContainerName(container),
            "localhost", 
            container.Ports.First(IsManagementPort).PublicPort);

    static MicroserviceName GetContainerName(ContainerListResponse container)
    {
        if (container.Labels.TryGetValue("com.docker.compose.service", out var dockerComposeName))
        {
            return dockerComposeName;
        }

        var name = container.Names.FirstOrDefault(_ => !string.IsNullOrWhiteSpace(_));
        if (name != null)
        {
            return name.TrimStart('/');
        }
        
        return MicroserviceName.NotSet;
    }

    static bool IsManagementPort(Port port)
        => port.Type == "tcp" && port.PrivatePort == new EndpointsConfiguration().Management.Port;
}
