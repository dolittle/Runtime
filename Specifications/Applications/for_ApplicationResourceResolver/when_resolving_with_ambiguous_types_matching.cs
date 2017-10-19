using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace doLittle.Runtime.Applications.Specs.for_ApplicationResourceResolver
{
    public class when_resolving_with_ambiguous_types_matching : given.no_resolvers
    {
        public class Hidden1
        {
            public class Implementation : IInterface { }
        }

        public class Hidden2
        {
            public class Implementation : IInterface { }
        }

        static Exception exception;

        Establish context = () =>
        {
            resource_type.SetupGet(t => t.Type).Returns(typeof(IInterface));
            type_finder.Setup(t => t.FindMultiple(typeof(IInterface))).Returns(
                new[] {
                    typeof(Hidden1.Implementation),
                    typeof(Hidden2.Implementation)
                });
        };

        Because of = () => exception = Catch.Exception(() => resolver.Resolve(identifier.Object));

        It should_throw_ambiguous_types = () => exception.ShouldBeOfExactType<AmbiguousTypes>();
    }
}
