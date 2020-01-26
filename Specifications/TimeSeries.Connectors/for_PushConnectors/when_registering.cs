// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.TimeSeries.Connectors.for_PushConnectors
{
    public class when_registering
    {
        static ConnectorId connector_id;
        static PushConnector push_connector;
        static PushConnectors push_connectors;

        Establish context = () =>
        {
            connector_id = Guid.NewGuid();
            push_connector = new PushConnector(connector_id, "Fourty Two");
            push_connectors = new PushConnectors(Mock.Of<ILogger>());
        };

        Because of = () => push_connectors.Register(push_connector);

        It should_consider_having_the_connector = () => push_connectors.Has(connector_id).ShouldBeTrue();
        It should_have_the_connector = () => push_connectors.GetById(connector_id).ShouldEqual(push_connector);
    }
}