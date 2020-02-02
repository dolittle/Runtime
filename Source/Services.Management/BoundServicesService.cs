// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Services.BoundServices;
using grpc = contracts::Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Services.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="BoundServicesBase"/> for working with <see cref="Service">services</see>
    /// that are bound in our system.
    /// </summary>
    public class BoundServicesService : BoundServicesBase
    {
        readonly IBoundServices _boundServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundServicesService"/> class.
        /// </summary>
        /// <param name="boundServices">Underlying <see cref="IBoundServices"/>.</param>
        public BoundServicesService(IBoundServices boundServices)
        {
            _boundServices = boundServices;
        }

        /// <inheritdoc/>
        public override async Task<grpc.Services> GetForServiceType(grpc.ServiceType request, ServerCallContext context)
        {
            var boundServices = _boundServices.GetFor(request.Name);
            var services = new grpc.Services();
            services.BoundServices.Add(boundServices.Select(_ => new grpc.Service { Name = _.Descriptor.FullName }));
            return await Task.FromResult(services).ConfigureAwait(false);
        }
    }
}