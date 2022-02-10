using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class yyyy : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("yyyy", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => string.IsNullOrEmpty(error).ShouldBeTrue();
}