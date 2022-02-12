// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.CLI.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Management.Contracts;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ManagementContracts = Dolittle.Runtime.Events.Processing.Management.Contracts;
using static Dolittle.Runtime.Events.Processing.Management.Contracts.Projections;
using UnpartitionedTenantScopedStreamProcessorStatus = Dolittle.Runtime.CLI.Runtime.Events.Processing.UnpartitionedTenantScopedStreamProcessorStatus;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Represents an implementation of <see cref="IManagementClient"/>.
/// </summary>
public class ManagementClient : IManagementClient
{
    readonly ICanCreateClients _clients;
    readonly IConvertProjectionDefinitions _definitionConverter;
    readonly IConvertStreamProcessorStatus _statusConverter;

    public ManagementClient(ICanCreateClients clients, IConvertProjectionDefinitions definitionConverter, IConvertStreamProcessorStatus statusConverter)
    {
        _clients = clients;
        _definitionConverter = definitionConverter;
        _statusConverter = statusConverter;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectionStatus>> GetAll(MicroserviceAddress runtime, TenantId tenant = null)
    {
        var client = _clients.CreateClientFor<ProjectionsClient>(runtime);

        var request = new GetAllProjectionsRequest
        {
            TenantId = tenant?.ToProtobuf()
        };

        var response = await client.GetAllAsync(request);
        if (response.Failure != null)
        {
            throw new GetAllProjectionsFailed(response.Failure.Reason);
        }

        return response.Projections.Select(CreateProjectionStatus);
    }

    ProjectionStatus CreateProjectionStatus(ManagementContracts.ProjectionStatus status)
        => new (
            status.ProjectionId.ToGuid(),
            status.ScopeId.ToGuid(),
            JsonConvert.DeserializeObject<JObject>(status.InitialState),
            _definitionConverter.ToRuntimeEventSelectors(status.Events),
            _definitionConverter.ToRuntimeCopySpecification(status.Copies),
            status.Alias,
            _statusConverter.Convert(status.Tenants).Cast<UnpartitionedTenantScopedStreamProcessorStatus>());
}
