// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Collections;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IEndpoints"/>.
    /// </summary>
    [Singleton]
    public class Endpoints : IEndpoints
    {
        readonly IDictionary<EndpointVisibility, List<IRepresentServiceType>> _serviceRepresentersForEndpointVisibility = new Dictionary<EndpointVisibility, List<IRepresentServiceType>>();
        readonly IDictionary<EndpointVisibility, IEndpoint> _endpoints = new Dictionary<EndpointVisibility, IEndpoint>();
        readonly IList<EndpointInfo> _endpointInfos = new List<EndpointInfo>();

        readonly EndpointsConfiguration _configuration;
        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly ILogger _logger;
        readonly IBoundServices _boundServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoints"/> class.
        /// </summary>
        /// <param name="serviceTypes">Instances of <see cref="IRepresentServiceType"/>.</param>
        /// <param name="configuration"><see cref="EndpointsConfiguration"/> for all endpoints.</param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for finding services to host.</param>
        /// <param name="container"><see cref="IContainer"/> for working with instances of host binders.</param>
        /// <param name="boundServices"><see cref="IBoundServices"/> for registering services that gets bound.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Endpoints(
            IInstancesOf<IRepresentServiceType> serviceTypes,
            EndpointsConfiguration configuration,
            ITypeFinder typeFinder,
            IContainer container,
            IBoundServices boundServices,
            ILogger logger)
        {
            _configuration = configuration;

            _serviceRepresentersForEndpointVisibility = serviceTypes.GroupBy(_ => _.Visibility)
                .ToDictionary(_ => _.Key, _ => _.ToList());

            _typeFinder = typeFinder;
            _container = container;
            _logger = logger;
            _boundServices = boundServices;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach ((_, var endpoint) in _endpoints) endpoint.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void Start()
        {
            _logger.LogDebug("Starting all endpoints");

            var servicesByVisibility = new Dictionary<EndpointVisibility, List<Service>>();

            foreach ((var type, var serviceTypeRepresenters) in _serviceRepresentersForEndpointVisibility)
            {
                var configuration = _configuration[type];
                if (configuration.Enabled)
                {
                    _logger.LogDebug("Preparing endpoint for {type} visibility - running on port {port}", type, configuration.Port);
                    var endpoint = GetEndpointFor(type);

                    serviceTypeRepresenters.ForEach(representer =>
                    {
                        var services = GetServicesForRepresenter(representer);
                        _boundServices.Register(representer.Identifier, services);

                        if (!servicesByVisibility.ContainsKey(type)) servicesByVisibility[type] = new List<Service>();
                        servicesByVisibility[type].AddRange(services);
                    });
                }
                else
                {
                    _logger.LogDebug("{type} endpoint is disabled", type);
                }
            }

            foreach ((var type, var endpoint) in _endpoints)
            {
                var configuration = _configuration[type];
                endpoint.Start(type, configuration, servicesByVisibility[type]);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<EndpointInfo> GetEndpoints() => _endpointInfos;

        IEndpoint GetEndpointFor(EndpointVisibility type)
        {
            if (_endpoints.ContainsKey(type)) return _endpoints[type];
            var endpoint = _container.Get<IEndpoint>();
            _endpoints[type] = endpoint;
            return endpoint;
        }

        IEnumerable<Service> GetServicesForRepresenter(IRepresentServiceType representer)
        {
            var services = new List<Service>();

            var binders = _typeFinder.FindMultiple(representer.BindingInterface);
            binders.ForEach(_ =>
            {
                _logger.LogDebug("Bind services from {implementation}", _.AssemblyQualifiedName);

                var binder = _container.Get(_) as ICanBindServices;

                var boundServices = binder.BindServices();
                boundServices.ForEach(service => _logger.LogTrace("Service : {serviceName}", service.Descriptor?.FullName ?? "Unknown"));

                services.AddRange(boundServices);
            });

            return services;
        }
    }
}
