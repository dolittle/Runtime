// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Types.for_ContractToImplementorsMap
{
    public class when_feeding_interface_type_first_and_implementations_two_subsequent_times : given.an_empty_map
    {
        Establish context = () => map.Feed(new[] { typeof(IInterface) });

        Because of = () =>
        {
            map.Feed(new[] { typeof(ImplementationOfInterface) });
            map.Feed(new[] { typeof(SecondImplementationOfInterface) });
        };

        It should_have_both_the_implementations_only = () => map.GetImplementorsFor(typeof(IInterface)).ShouldContainOnly(typeof(ImplementationOfInterface), typeof(SecondImplementationOfInterface));
    }
}
