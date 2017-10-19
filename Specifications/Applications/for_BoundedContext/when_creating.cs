using Machine.Specifications;

namespace doLittle.Runtime.Applications.Specs.for_BoundedContext
{
    public class when_creating
    {
        const string name = "Some Bounded Context";
        static BoundedContext bounded_context;

        Because of = () => bounded_context = new BoundedContext(name);

        It should_have_the_name = () => ((string)bounded_context.Name).ShouldEqual(name);
    }
}
