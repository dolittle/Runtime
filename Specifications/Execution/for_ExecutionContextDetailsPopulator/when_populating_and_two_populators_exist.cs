using System.Dynamic;
using Dolittle.DependencyInversion;
using Dolittle.Types;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Execution.Specs.for_ExecutionContextDetailsPopulator
{
    public class when_populating_and_two_populators_exist
    {
        static Mock<ITypeFinder> type_finder;
        static ExecutionContextDetailsPopulator populator;
        static Mock<IExecutionContext> execution_context;
        static Mock<IContainer> container;
        static ExpandoObject details;

        static Mock<ICanPopulateExecutionContextDetails>    first_populator;
        static Mock<ICanPopulateExecutionContextDetails>    second_populator;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            execution_context = new Mock<IExecutionContext>();
            details = new ExpandoObject();
            container = new Mock<IContainer>();

            type_finder.Setup(t => t.FindMultiple<ICanPopulateExecutionContextDetails>()).Returns(new[] { typeof(string), typeof(object) });

            first_populator = new Mock<ICanPopulateExecutionContextDetails>();
            second_populator = new Mock<ICanPopulateExecutionContextDetails>();

            container.Setup(c => c.Get(typeof(string))).Returns(first_populator.Object);
            container.Setup(c => c.Get(typeof(object))).Returns(second_populator.Object);

            populator = new ExecutionContextDetailsPopulator(type_finder.Object, container.Object);
        };

        Because of = () => populator.Populate(execution_context.Object, details);

        It should_call_populate_on_first_populator = () => first_populator.Verify(p => p.Populate(execution_context.Object, details));
        It should_call_populate_on_second_populator = () => second_populator.Verify(p => p.Populate(execution_context.Object, details));
    }
}