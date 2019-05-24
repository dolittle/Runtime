
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_getting_type_for_date_time
    {
        static Types type;

        Because of = () => type = DateTime.Now.GetProtobufType();

        It should_be_date_time = () => type.ShouldEqual(Types.DateTime);
    }    
}