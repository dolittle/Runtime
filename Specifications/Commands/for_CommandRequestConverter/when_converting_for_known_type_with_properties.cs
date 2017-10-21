using System.Collections.Generic;
using doLittle.Runtime.Applications;
using doLittle.Commands;
using doLittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace doLittle.Runtime.Commands.Specs.for_CommandRequestConverter
{
    public class when_converting_for_known_type_with_properties : given.all_dependencies
    {
        const string string_property_name = "StringProperty";
        const string string_property_value = "Some string";
        const string int_property_name = "IntProperty";
        const int int_property_value = 42;

        static CommandRequestConverter converter;
        static CommandRequest request;
        static Command result;
        

        Establish context = () =>
        {
            var identifier = new Mock<IApplicationResourceIdentifier>();
            var content = new Dictionary<string, object>()
            {
                { string_property_name, string_property_value },
                { int_property_name, int_property_value }
            };

            converter = new CommandRequestConverter(application_resource_resolver.Object);
            request = new CommandRequest(
                    TransactionCorrelationId.NotSet,
                    identifier.Object,
                    content
                );

            application_resource_resolver.Setup(_ => _.Resolve(identifier.Object)).Returns(typeof(Command));
        };

        Because of = () => result = converter.Convert(request) as Command;

        It should_return_a_command_of_expected_type = () => result.ShouldNotBeNull();
        It should_set_string_property = () => result.StringProperty.ShouldEqual(string_property_value);
        It should_set_int_property = () => result.IntProperty.ShouldEqual(int_property_value);
    }
}
