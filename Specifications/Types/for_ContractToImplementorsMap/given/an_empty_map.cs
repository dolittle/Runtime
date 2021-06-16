// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Types.for_ContractToImplementorsMap.given
{
    public class an_empty_map
    {
        protected static ContractToImplementorsMap map;

        Establish context = () => map = new ContractToImplementorsMap();
    }
}
