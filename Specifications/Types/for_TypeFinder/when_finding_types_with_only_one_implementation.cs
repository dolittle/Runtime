// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Types.for_TypeFinder;

[Subject(typeof(TypeFinder))]
public class when_finding_types_with_only_one_implementation : given.a_type_finder
{
    static Type type_found;

    Establish context = () => contract_to_implementors_map_mock.Setup(c => c.GetImplementorsFor(typeof(ISingle))).Returns(new[] { typeof(Single) });

    Because of = () => type_found = type_finder.FindSingle<ISingle>();

    It should_not_return_null = () => type_found.ShouldNotBeNull();
    It should_return_correct_implementation_when = () => type_found.ShouldEqual(typeof(Single));
}