// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_getting_partition_id;

public class and_partition_id_is_set
{
    static PartitionId partition;
    static PartitionId result;

    Establish context = () => partition = "a partition";

    Because of = () => result = new ConsumerSubscriptionRequest
    {
        PartitionId = partition,
        PartitionIdLegacy = new Guid("2bed04ca-f892-4cb5-8e9d-9e4b18601df0").ToProtobuf()
    }.GetPartitionId();

    It should_get_the_partition_id = () => result.Should().Be(partition);
}