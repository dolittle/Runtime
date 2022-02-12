// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Projections.for_ValidateOccurredFormat.when_validating.given;

public class all_dependencies
{
    protected static ValidateOccurredFormat validator;
    protected static bool is_valid;
    protected static Exception error;
    Establish context = () =>
    {
        validator = new ValidateOccurredFormat();
    };
}