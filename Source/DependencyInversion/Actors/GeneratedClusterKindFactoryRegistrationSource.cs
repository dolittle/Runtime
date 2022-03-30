// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.DependencyInversion.Actors;

/// <summary>
/// Represents an implementation of <see cref="IRegistrationSource"/> that creates registrations for dependencies of type <see cref="Func{TResult}"/>
/// where the first parameter is a <see cref="IContext"/> and second is <see cref="ClusterIdentity"/>, by delegating the resolving to the per-tenant containers.
/// </summary>
public class GeneratedClusterKindFactoryRegistrationSource : IRegistrationSource
{
    /// <inheritdoc />
    public bool IsAdapterForIndividualComponents => false;

    /// <inheritdoc />
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
        if (!IsDelegateWithIContextAndClusterIdentityAsFirstAndSecondParameter(service, out var delegateType, out var parameters, out var grainType))
        {
            return Enumerable.Empty<IComponentRegistration>();
        }

        Func<IContext, ClusterIdentity, IServiceProvider, object> resolveGrain =
            (context, clusterIdentity, provider) =>
            {
                if (Attribute.IsDefined(grainType, typeof(PerTenantAttribute)))
                {
                    provider = provider.GetRequiredService<ITenantServiceProviders>().ForTenant(clusterIdentity.Identity);
                }
                //TODO: To create the virtual actor grain we need the context and cluster identity, but the delegate to resolve cannot be Func<IContext, ClusterIdentity, TGrain>
                // because it would cause endless recursion. That's why I put the int as the first type parameter
                var getGrainDelegateToResolve = Expression.GetDelegateType(typeof(int), typeof(IContext), typeof(ClusterIdentity), grainType);
                var getGrain = provider.GetRequiredService(getGrainDelegateToResolve) as Delegate;
                return getGrain!.DynamicInvoke(0, context, clusterIdentity);
            };

        var generatedDelegateParameters = parameters.Select(_ => Expression.Parameter(_.ParameterType, _.Name)).ToArray();
        var contextParameter = generatedDelegateParameters[0];
        var clusterIdentityParameter = generatedDelegateParameters[1];

        return new[]
        {
            RegistrationBuilder
                .ForDelegate(delegateType, (context, _) => Expression.Lambda(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Constant(resolveGrain.Target),
                            resolveGrain.Method,
                            contextParameter,
                            clusterIdentityParameter,
                            Expression.Constant(context.Resolve<IServiceProvider>())),
                        grainType),
                    generatedDelegateParameters
                ).Compile())
                .InstancePerLifetimeScope() //TODO: This should work, but I'm not 100% sure
                .ExternallyOwned()
                .As(service)
                .CreateRegistration(),
        };
    }
    
    static bool IsDelegateWithIContextAndClusterIdentityAsFirstAndSecondParameter(Service service, out Type delegateType, out ParameterInfo[] parameters, out Type returnType)
    {
        delegateType = default;
        parameters = default;
        returnType = default;
        
        if (service is not IServiceWithType serviceWithType)
        {
            return false;
        }

        delegateType = serviceWithType.ServiceType;
        if (!typeof(Delegate).IsAssignableFrom(delegateType))
        {
            return false;
        }

        var invokeMethod = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        if (invokeMethod == default)
        {
            return false;
        }

        parameters = invokeMethod.GetParameters();
        if (parameters.Length < 2 || parameters[0].ParameterType != typeof(IContext) || parameters[1].ParameterType != typeof(ClusterIdentity))
        {
            return false;
        }
        
        returnType = invokeMethod.ReturnType;
        
        return returnType != typeof(void);
    }
}
