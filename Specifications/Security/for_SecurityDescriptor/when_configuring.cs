// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor;

public class when_configuring
{
    static SecurityDescriptor descriptor;

    Because of = () => descriptor = new SecurityDescriptor();

    It should_have_a_when_clause_set = () => descriptor.When.ShouldNotBeNull();
}