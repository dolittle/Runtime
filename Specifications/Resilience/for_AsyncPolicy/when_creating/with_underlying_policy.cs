// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_AsyncPolicy.when_creating;

public class with_underlying_policy
{
    static Polly.IAsyncPolicy underlying_policy;
    static AsyncPolicy policy;

    Establish context = () => underlying_policy = Polly.Policy.NoOpAsync();

    Because of = () => policy = new AsyncPolicy(underlying_policy);

    It should_note_have_delegated_policy = () => policy.DelegatedPolicy.ShouldBeNull();
    It should_have_underlying_policy = () => policy.UnderlyingPolicy.ShouldNotBeNull();
    It should_have_the_correct_underlying_policy = () => policy.UnderlyingPolicy.ShouldEqual(underlying_policy);
}