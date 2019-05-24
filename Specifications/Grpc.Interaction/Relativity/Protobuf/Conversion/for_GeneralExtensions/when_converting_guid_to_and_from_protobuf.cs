
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_converting_guid_to_and_from_protobuf
    {
        static Guid guid;
        static System.Protobuf.guid protobuf;
        static Guid result;

        Establish context = () => guid = Guid.NewGuid();

        Because of = () => 
        {
            protobuf = guid.ToProtobuf();
            result = protobuf.ToGuid();
        };

        It should_be_equal_to_the_original = () => result.ShouldEqual(guid);
    }
}