// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Extension.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_CallInvokerManager.given
{
    public class a_call_invoker_manager
    {
        protected static CallInvokerManager manager;
        protected static ClientEndpointsConfiguration configuration;
        protected static Mock<IKnownClients> known_clients;
        protected static Mock<IMetadataProviders> metadata_providers;

        Establish context = () =>
        {
            known_clients = new Mock<IKnownClients>();
            metadata_providers = new Mock<IMetadataProviders>();
            configuration = new ClientEndpointsConfiguration(new Dictionary<EndpointVisibility, ClientEndpointConfiguration>
            {
                { EndpointVisibility.Public, new ClientEndpointConfiguration("localhost", 1) },
                { EndpointVisibility.Private, new ClientEndpointConfiguration("localhost", 1) }
            });
            manager = new CallInvokerManager(known_clients.Object, configuration, metadata_providers.Object, Mock.Of<ILogger>());
        };
    }
}