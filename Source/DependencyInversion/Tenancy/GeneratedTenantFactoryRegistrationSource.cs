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
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dolittle.Runtime.DependencyInversion.Tenancy;

/// <summary>
/// Represents an implementation of <see cref="IRegistrationSource"/> that creates registrations for dependencies of type <see cref="Func{TResult}"/>
/// where the first parameter is a <see cref="TenantId"/>, by delegating the resolving to the per-tenant containers.
/// </summary>
public class GeneratedTenantFactoryRegistrationSource : IRegistrationSource
{
    /// <inheritdoc />
    public bool IsAdapterForIndividualComponents => false;

    /// <inheritdoc />
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
        if (!IsDelegateWithTenantIdAsFirstParameter(service, out var delegateType, out var parameters, out var resolvedType))
        {
            return Enumerable.Empty<IComponentRegistration>();
        }

        var tenantDelegateArguments = parameters.Skip(1).Select(_ => _.ParameterType).Append(resolvedType).ToArray();
        var tenantDelegateToResolve = Expression.GetDelegateType(tenantDelegateArguments);

        Func<TenantId, ITenantServiceProviders, object[], object> resolveServiceForTenant = 
            (tenant, providers, arguments) =>
            {
                var provider = providers.ForTenant(tenant);
                var resolver = provider.GetRequiredService(tenantDelegateToResolve) as Delegate;
                return resolver?.DynamicInvoke(arguments); //TODO: Null check?
            };

        var generatedDelegateParameters = parameters.Select(_ => Expression.Parameter(_.ParameterType, _.Name)).ToList();
        var tenantIdParameter = generatedDelegateParameters.First();
        var tenantDelegateParameters = Expression.NewArrayInit(typeof(object), generatedDelegateParameters.Skip(1));

        return new[]
        {
            RegistrationBuilder
                .ForDelegate(delegateType, (context, _) =>
                {
                    var serviceProviders = context.Resolve<ITenantServiceProviders>();
                    return Expression.Lambda(
                        Expression.Convert(
                            Expression.Call(
                                Expression.Constant(resolveServiceForTenant.Target),
                                resolveServiceForTenant.Method,
                                tenantIdParameter,
                                Expression.Constant(serviceProviders),
                                tenantDelegateParameters),
                            resolvedType),
                        generatedDelegateParameters
                    ).Compile();
                })
                .InstancePerLifetimeScope() //TODO: This should work, but I'm not 100% sure
                .ExternallyOwned()
                .As(service)
                .CreateRegistration(),
        };
    }

    static bool IsDelegateWithTenantIdAsFirstParameter(Service service, out Type delegateType, out ParameterInfo[] parameters, out Type returnType)
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
        if (parameters.Length < 1 || parameters[0].ParameterType != typeof(TenantId))
        {
            return false;
        }
        
        returnType = invokeMethod.ReturnType;
        
        return returnType != typeof(void);
    }
}
