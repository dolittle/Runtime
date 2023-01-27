// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRoots.when_registering;

public class an_aggregate_root_that_is_not_already_registered : given.all_dependencies
{
    static AggregateRoot _oldRoot;

    Establish context = () =>
    {
        _oldRoot = an_aggregate_root with
        {
            Identifier = an_aggregate_root.Identifier with
            {
                Id = "4cd25224-8258-4405-a9db-2e313c9ac4c0"
            }
        };
        aggregate_roots.Register(_oldRoot);
    };
        
    Because of = () => aggregate_roots.Register(an_aggregate_root);

    It should_only_have_the_old_and_new_aggregate_roots = () => aggregate_roots.All.ShouldContainOnly(_oldRoot, an_aggregate_root);
}