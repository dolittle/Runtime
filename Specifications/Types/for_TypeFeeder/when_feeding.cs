// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.Runtime.Assemblies;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Types.for_TypeFeeder
{
    [Subject(typeof(TypeFeeder))]
    public class when_feeding
    {
        protected static Type[] types;
        static Mock<Assembly> assembly_mock;
        static Mock<IAssemblies> assemblies_mock;
        static Mock<IContractToImplementorsMap> contract_to_implementors_map_mock;

        static TypeFeeder type_feeder;

        Establish context = () =>
        {
            types = new[]
            {
                typeof(ISingle),
                typeof(Single),
                typeof(IMultiple),
                typeof(FirstMultiple),
                typeof(SecondMultiple)
            };

            assembly_mock = new Mock<Assembly>();
            assembly_mock.Setup(a => a.GetTypes()).Returns(types);
            assembly_mock.Setup(a => a.FullName).Returns("A.Full.Name");

            assemblies_mock = new Mock<IAssemblies>();
            assemblies_mock.Setup(x => x.GetAll()).Returns(new[] { assembly_mock.Object });

            contract_to_implementors_map_mock = new Mock<IContractToImplementorsMap>();
            contract_to_implementors_map_mock.SetupGet(c => c.All).Returns(types);

            type_feeder = new TypeFeeder(Mock.Of<ILogger>());
        };

        Because of = () => type_feeder.Feed(assemblies_mock.Object, contract_to_implementors_map_mock.Object);

        It should_populate_map = () => contract_to_implementors_map_mock.Verify(c => c.Feed(types), Times.Once);
    }
}