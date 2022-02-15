// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor.given;

public class a_configured_security_descriptor
{
    protected static SecurityDescriptor security_descriptor;
    protected static SomeType type_that_has_namespace_and_type_rule;
    protected static SomeOtherType type_that_has_namespace_rule;
    protected static DifferentNamespace.TypeInDifferentNamespace type_that_is_not_applicable;
    protected static Mock<ICanResolvePrincipal> resolve_principal_mock;

    Establish context = () =>
    {
        resolve_principal_mock = new Mock<ICanResolvePrincipal>();
        security_descriptor = new SecurityDescriptor();
        type_that_has_namespace_and_type_rule = new SomeType();
        type_that_has_namespace_rule = new SomeOtherType();
        type_that_is_not_applicable = new DifferentNamespace.TypeInDifferentNamespace();
    };
}