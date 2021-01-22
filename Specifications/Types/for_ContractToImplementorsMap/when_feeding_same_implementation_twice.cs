// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Types.for_ContractToImplementorsMap
{
    public class when_feeding_same_implementation_twice : given.an_empty_map
    {
        Establish context = () => map.Feed(new[] { typeof(ImplementationOfInterface) });

        Because of = () => map.Feed(new[] { typeof(ImplementationOfInterface) });

        It should_only_return_one_implementor = () => map.GetImplementorsFor(typeof(IInterface)).Count().ShouldEqual(1);
    }
}
