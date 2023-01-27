// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_setting_partition_id;

public class and_partition_id_is_guid
{
    static PartitionId partition;
    static ConsumerSubscriptionRequest request;

    Establish context = () =>
    {
        partition = "3f0a1e6f-02a4-4b21-ba58-cc147e9c0b87";
        request = new ConsumerSubscriptionRequest
        {
            PartitionId = partition
        };
    };

    Because of = () => request.TrySetPartitionIdLegacy();

    It should_set_legacy_partition_id = () => request.PartitionIdLegacy.Should().Be(new Guid(partition).ToProtobuf());
}