// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IBoundServices"/>.
    /// </summary>
    [Singleton]
    public class BoundServices : IBoundServices
    {
        readonly ConcurrentDictionary<ServiceType, List<Service>> _servicesPerServiceType = new();
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundServices"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public BoundServices(ILogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Register(ServiceType type, IEnumerable<Service> services)
        {
            services.ForEach(service => _logger.RegisteringBoundService(service.Descriptor?.Name ?? "unknown"));

            if (!_servicesPerServiceType.ContainsKey(type)) _servicesPerServiceType[type] = new List<Service>();
            _servicesPerServiceType[type].AddRange(services);
        }

        /// <inheritdoc/>
        public bool HasFor(ServiceType type) =>
            _servicesPerServiceType.ContainsKey(type);

        /// <inheritdoc/>
        public IEnumerable<Service> GetFor(ServiceType type)
        {
            ThrowIfUnknownServiceType(type);

            return _servicesPerServiceType[type];
        }

        void ThrowIfUnknownServiceType(ServiceType type)
        {
            if (!_servicesPerServiceType.ContainsKey(type)) throw new UnknownServiceType(type);
        }
    }
}