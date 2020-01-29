// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Connectors.for_PullConnectors
{
    public class when_unregistering
    {
        static ConnectorId connector_id;
        static PullConnector pull_connector;
        static PullConnectors pull_connectors;

        Establish context = () =>
        {
            connector_id = Guid.NewGuid();
            pull_connector = new PullConnector(connector_id, "Fourty Two", 42);
            pull_connectors = new PullConnectors(Mock.Of<ILogger>());
            pull_connectors.Register(pull_connector);
        };

        Because of = () => pull_connectors.Unregister(pull_connector);

        It should_not_consider_having_the_connector = () => pull_connectors.Has(connector_id).ShouldBeFalse();
    }
}