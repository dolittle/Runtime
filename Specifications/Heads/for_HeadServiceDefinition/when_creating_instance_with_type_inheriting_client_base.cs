/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Heads.for_HeadServiceDefinition
{
    public class when_creating_instance_with_type_inheriting_client_base
    {
        class my_client : ClientBase<my_client> {
            protected override my_client NewInstance(ClientBaseConfiguration configuration) 
            {
                throw new NotImplementedException();
            }
        }

        static HeadServiceDefinition result;

        Because of = () => result = new HeadServiceDefinition(typeof(my_client), null);

        It should_create_an_instance = () => result.ShouldNotBeNull();
    }    
}