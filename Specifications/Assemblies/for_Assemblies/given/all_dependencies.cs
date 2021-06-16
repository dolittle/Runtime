// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Assemblies;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Assemblies.for_Assemblies.given
{
    public class all_dependencies
    {
        protected static Mock<IAssemblyProvider> assembly_provider_mock;

        Establish context = () => assembly_provider_mock = new Mock<IAssemblyProvider>();
    }
}