using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class null_string : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid(null, out error);

    It should_not_be_valid = () => is_valid.ShouldBeFalse();
    It should_output_error = () => string.IsNullOrEmpty(error).ShouldBeFalse();
}