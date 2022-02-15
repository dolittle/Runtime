// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.Resilience;

/// <summary>
/// Represents the bindings for all <see cref="IPolicyFor{T}"/>.
/// </summary>
public class Bindings : ICanProvideBindings
{
    readonly GetContainer _getContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bindings"/> class.
    /// </summary>
    /// <param name="getContainer"><see cref="GetContainer"/> for getting the <see cref="IContainer"/>.</param>
    public Bindings(GetContainer getContainer)
    {
        _getContainer = getContainer;
    }

    /// <inheritdoc/>
    public void Provide(IBindingProviderBuilder builder)
    {
        BindPolicyFor(builder);
        BindAsyncPolicyFor(builder);
    }

    void BindPolicyFor(IBindingProviderBuilder builder)
    {
        builder.Bind(typeof(IPolicyFor<>)).To((BindingContext context) =>
        {
            var policies = _getContainer().Get<IPolicies>();

            Expression<Func<IPolicyFor<object>>> getForExpression = () => policies.GetFor<object>();
            var methodInfo = getForExpression.GetMethodInfo().GetGenericMethodDefinition();
            var target = context.Service.GenericTypeArguments[0];
            var genericMethodInfo = methodInfo.MakeGenericMethod(target);
            var result = genericMethodInfo.Invoke(policies, null);
            return result;
        });
    }

    void BindAsyncPolicyFor(IBindingProviderBuilder builder)
    {
        builder.Bind(typeof(IAsyncPolicyFor<>)).To((BindingContext context) =>
        {
            var policies = _getContainer().Get<IPolicies>();

            Expression<Func<IAsyncPolicyFor<object>>> getForExpression = () => policies.GetAsyncFor<object>();
            var methodInfo = getForExpression.GetMethodInfo().GetGenericMethodDefinition();
            var target = context.Service.GenericTypeArguments[0];
            var genericMethodInfo = methodInfo.MakeGenericMethod(target);
            var result = genericMethodInfo.Invoke(policies, null);
            return result;
        });
    }
}