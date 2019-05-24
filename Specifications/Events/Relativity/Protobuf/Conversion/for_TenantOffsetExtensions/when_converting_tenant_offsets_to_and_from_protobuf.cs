/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_TenantOffsetExtensions
{
    public class when_converting_tenant_offsets_to_and_from_protobuf
    {
        static IEnumerable<TenantOffset> original;
        static RepeatedField<Runtime.Grpc.Interaction.Protobuf.TenantOffset> protobuf;
        static IEnumerable<TenantOffset> result;

        Establish context = () => original = new[] {
            new TenantOffset(Guid.NewGuid(),42),
            new TenantOffset(Guid.NewGuid(),43)
        };

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToTenantOffsets();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}