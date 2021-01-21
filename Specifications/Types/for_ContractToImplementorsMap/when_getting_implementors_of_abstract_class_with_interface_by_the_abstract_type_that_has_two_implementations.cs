// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Types.for_ContractToImplementorsMap
{
    public class when_getting_implementors_of_abstract_class_with_interface_by_the_abstract_type_that_has_two_implementations : given.an_empty_map
    {
        static IEnumerable<Type> result;

        Establish context = () => map.Feed(new[] { typeof(ImplementationOfAbstractClassWithInterface), typeof(SecondImplementationOfAbstractClassWithInterface) });

        Because of = () => result = map.GetImplementorsFor(typeof(AbstractClass));

        It should_have_the_implementations_only = () => result.ShouldContainOnly(typeof(ImplementationOfAbstractClassWithInterface), typeof(SecondImplementationOfAbstractClassWithInterface));
    }
}
