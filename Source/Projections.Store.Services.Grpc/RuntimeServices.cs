// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// runtime service implementations for Heads.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly ProjectionsGrpcService _eventStoreGrpcService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="eventStoreService">The <see cref="ProjectionsService"/>.</param>
        public RuntimeServices(ProjectionsGrpcService eventStoreService)
        {
            _eventStoreGrpcService = eventStoreService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Projections";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new Service(_eventStoreGrpcService, Contracts.Projections.BindService(_eventStoreGrpcService), Contracts.Projections.Descriptor)
            };
    }
}
