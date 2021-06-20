using System.Reflection.PortableExecutable;
using System.Linq;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Processing;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using System;

namespace Dolittle.Runtime.EventHorizon.for_EventHorizonConsentsConfiguration
{
    public class when_creating_a_configuration
    {
        static TenantId producer_tenant;
        static Microservice consumer_microservice;
        static TenantId consumer_tenant;
        static StreamId stream;
        static PartitionId partition;
        static ConsentId consent;
        static EventHorizonConsentsConfiguration result;

        Establish context = () =>
        {
            producer_tenant = Guid.Parse("75230eee-2620-4a38-b69c-c07a0bb5e0fa");
            consumer_microservice = Guid.Parse("07b697ed-44e4-4167-805f-ff5a9851cf98");
            consumer_tenant = Guid.Parse("8e18214b-287f-41a1-9fef-dd67ef0cf86f");
            stream = Guid.Parse("82695d13-f7a2-4755-ba24-a473154ab7a3");
            partition = Guid.Parse("1221f439-7b6c-4f34-b084-856e52974f67");
            consent = Guid.Parse("b2c2f59c-8172-4134-a9d1-a6288d3baed3");
        };

        Because of = () => result = new EventHorizonConsentsConfiguration(new Dictionary<Guid, IEnumerable<EventHorizonConsentConfiguration>>()
            {

                {
                    producer_tenant,
                    new[]
                    {
                        new EventHorizonConsentConfiguration(consumer_microservice, consumer_tenant, stream, partition, consent)
                    }
                }
            });

        It should_only_have_entry_for_producer_tenant = () => result.Keys.ShouldContainOnly(producer_tenant);
        It should_have_one_consent_configuration = () => result[producer_tenant].Count().ShouldEqual(1);
        It should_have_correct_consumer_microservice = () => result[producer_tenant].First().Microservice.ShouldEqual(consumer_microservice.Value);
        It should_have_correct_consumer_tenant = () => result[producer_tenant].First().Tenant.ShouldEqual(consumer_tenant.Value);
        It should_have_correct_stream = () => result[producer_tenant].First().Stream.ShouldEqual(stream.Value);
        It should_have_correct_partition = () => result[producer_tenant].First().Partition.ShouldEqual(partition.Value);
        It should_have_correct_consent = () => result[producer_tenant].First().Consent.ShouldEqual(consent.Value);
        It should_get_correct_consent_configuration = () =>
        {
            var config = result.GetConsentConfigurationsFor(producer_tenant);
            config.ShouldNotBeEmpty();
            config.Count().ShouldEqual(1);
            var item = config.First();
            item.Microservice.ShouldEqual(consumer_microservice);
            item.Tenant.ShouldEqual(consumer_tenant);
            item.Stream.ShouldEqual(stream);
            item.Partition.ShouldEqual(partition);
            item.Consent.ShouldEqual(consent);
        };
    }
}
