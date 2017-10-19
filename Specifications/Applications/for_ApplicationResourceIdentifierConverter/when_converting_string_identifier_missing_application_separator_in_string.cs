using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace doLittle.Runtime.Applications.Specs.for_ApplicationResourceIdentifierConverter
{
    public class when_converting_string_identifier_missing_application_separator_in_string : given.an_application_resource_identifier_converter
    {
        static string string_identifier = "Application";
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => converter.FromString(string_identifier));

        It should_throw_unable_to_identify_application = () => exception.ShouldBeOfExactType<UnableToIdentifyApplication>();
    }
}
