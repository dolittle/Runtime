// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.DependencyInversion.for_BindingScopeBuilder.given
{
    public class a_transient_binding
    {
        protected static Binding binding;
        protected static BindingScopeBuilder builder;
        protected static Mock<IActivationStrategy> activation_strategy;

        Establish context = () =>
        {
            activation_strategy = new Mock<IActivationStrategy>();
            activation_strategy.Setup(_ => _.GetTargetType()).Returns(typeof(object));
            binding = new Binding(typeof(object), activation_strategy.Object, new Scopes.Transient());
            builder = new BindingScopeBuilder(binding);
        };
    }
}