// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_getting_partition_id;

public class and_partition_id_is_not_set
{
    static PartitionId partition;
    static PartitionId result;

    Establish context = () => partition = "3f0a1e6f-02a4-4b21-ba58-cc147e9c0b87";

    Because of = () => result = new ConsumerSubscriptionRequest
    {
        PartitionIdLegacy = new Guid(partition).ToProtobuf()
    }.GetPartitionId();

    It should_get_the_legacy_partition_id = () => result.Should().Be(partition);
}