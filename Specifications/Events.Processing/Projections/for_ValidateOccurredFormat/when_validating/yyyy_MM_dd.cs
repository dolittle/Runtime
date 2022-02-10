using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating;

public class yyyy_MM_dd : given.all_dependencies
{
    Because of = () => is_valid = validator.IsValid("yyyy-MM-dd", out error);

    It should_be_valid = () => is_valid.ShouldBeTrue();
    It should_not_output_error = () => error.ShouldBeNull();
}