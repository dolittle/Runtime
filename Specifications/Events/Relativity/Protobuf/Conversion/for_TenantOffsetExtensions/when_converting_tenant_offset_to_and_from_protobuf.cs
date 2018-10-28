/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_TenantOffsetExtensions
{
    public class when_converting_tenant_offset_to_and_from_protobuf
    {
        static Dolittle.Runtime.Events.Relativity.TenantOffset original;
        static TenantOffset protobuf;
        static Dolittle.Runtime.Events.Relativity.TenantOffset result;

        Establish context = () => original = new Dolittle.Runtime.Events.Relativity.TenantOffset(Guid.NewGuid(),42);

        Because of = () => 
        {
            protobuf = original.ToProtobuf();
            result = protobuf.ToTenantOffset();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(original);
    }
}