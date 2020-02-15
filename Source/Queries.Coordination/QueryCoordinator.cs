// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Queries.Security;
using Dolittle.Queries.Validation;
using Dolittle.ReadModels;
using Dolittle.Types;

namespace Dolittle.Queries.Coordination
{
    /// <summary>
    /// Represents an implementation of <see cref="IQueryProvider"/>.
    /// </summary>
    public class QueryCoordinator : IQueryCoordinator
    {
        const string QueryPropertyName = "Query";
        const string ExecuteMethodName = "Execute";

        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly IFetchingSecurityManager _fetchingSecurityManager;
        readonly IQueryValidator _validator;
        readonly IReadModelFilters _filters;
        readonly ILogger _logger;
        Dictionary<Type, Type> _queryProviderTypesPerTargetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCoordinator"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for discovering <see cref="IQueryProviderFor{T}"/> implementations.</param>
        /// <param name="container"><see cref="IContainer"/> for getting instances of <see cref="IQueryProviderFor{T}">query providers</see>.</param>
        /// <param name="fetchingSecurityManager"><see cref="IFetchingSecurityManager"/> to use for securing <see cref="IQuery">queries</see>.</param>
        /// <param name="validator"><see cref="IQueryValidator"/> to use for validating <see cref="IQuery">queries</see>.</param>
        /// <param name="filters"><see cref="IReadModelFilters">Filters</see> used to filter any of the read models coming back after a query.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public QueryCoordinator(
            ITypeFinder typeFinder,
            IContainer container,
            IFetchingSecurityManager fetchingSecurityManager,
            IQueryValidator validator,
            IReadModelFilters filters,
            ILogger logger)
        {
            _typeFinder = typeFinder;
            _container = container;
            _fetchingSecurityManager = fetchingSecurityManager;
            _validator = validator;
            _filters = filters;
            _logger = logger;
            DiscoverQueryTypesPerTargetType();
        }

        /// <inheritdoc/>
        public Task<QueryResult> Execute(IQuery query, PagingInfo paging)
        {
            var queryType = query.GetType();
            _logger.Debug($"Executing query of type '{queryType.AssemblyQualifiedName}'");

            var taskCompletionSource = new TaskCompletionSource<QueryResult>();
            var result = QueryResult.For(query);

            try
            {
                ThrowIfNoQueryPropertyOnQuery(queryType);

                var authorizationResult = _fetchingSecurityManager.Authorize(query);
                if (!authorizationResult.IsAuthorized)
                {
                    result.SecurityMessages = authorizationResult.BuildFailedAuthorizationMessages();
                    result.Items = Array.Empty<object>();
                    taskCompletionSource.SetResult(result);
                    return taskCompletionSource.Task;
                }

                var validation = _validator.Validate(query);
                result.BrokenRules = validation.BrokenRules;
                if (!validation.Success)
                {
                    result.Items = Array.Empty<object>();
                    taskCompletionSource.SetResult(result);
                    return taskCompletionSource.Task;
                }

                var property = GetQueryPropertyFromQuery(queryType);
                var actualQuery = property.GetValue(query, null);
                var queryReturnType = actualQuery.GetType();
                if (actualQuery is Task && queryReturnType.IsGenericType)
                {
                    var task = actualQuery as Task;
#pragma warning disable CA2008
                    task.ContinueWith(tr =>
                    {
                        var resultProperty = tr.GetType().GetProperty("Result");
                        queryReturnType = queryReturnType.GetGenericArguments()[0];
                        actualQuery = resultProperty.GetValue(tr);
                        HandleActualQuery(queryType, actualQuery, queryReturnType, result, paging);

                        taskCompletionSource.SetResult(result);
                    });
#pragma warning restore CA2008
                }
                else
                {
                    HandleActualQuery(queryType, actualQuery, queryReturnType, result, paging);
                    taskCompletionSource.SetResult(result);
                }
            }
            catch (TargetInvocationException ex)
            {
                result.Exception = ex.InnerException;
                taskCompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                result.Exception = ex;

                switch (ex)
                {
                    case MissingQueryProperty missingQueryProperty:
                        {
                            taskCompletionSource.SetException(ex);
                            throw;
                        }

                    default:
                        {
                            taskCompletionSource.SetResult(result);
                        }

                        break;
                }
            }

            return taskCompletionSource.Task;
        }

        void HandleActualQuery(Type queryType, object actualQuery, Type resultType, QueryResult result, PagingInfo paging)
        {
            var provider = GetQueryProvider(queryType, resultType, actualQuery);
            var providerResult = ExecuteOnProvider(provider, actualQuery, paging);
            result.TotalItems = providerResult.TotalItems;
            result.Items = providerResult.Items is IEnumerable<IReadModel> readModels ? _filters.Filter(readModels) : providerResult.Items;

            _logger.Trace($"Query resulted in {result.TotalItems} items");
        }

        void ThrowIfNoQueryPropertyOnQuery(Type queryType)
        {
            var property = GetQueryPropertyFromQuery(queryType);
            if (property == null)
                throw new MissingQueryProperty(queryType);
        }

        void ThrowIfMissingQueryProvider(Type queryProviderType, Type queryType, Type resultType)
        {
            if (queryProviderType == null) throw new MissingQueryProvider(queryType, resultType);
        }

        PropertyInfo GetQueryPropertyFromQuery(Type queryType)
        {
            var property = queryType.GetTypeInfo().GetProperty(QueryPropertyName, BindingFlags.Public | BindingFlags.Instance);
            return property;
        }

        object GetQueryProvider(Type queryType, Type resultType, object actualQuery)
        {
            Type queryProviderType = null;
            if (actualQuery != null && actualQuery.GetType() != resultType)
                queryProviderType = GetActualProviderTypeFrom(actualQuery.GetType());

            if (queryProviderType == null)
                queryProviderType = GetActualProviderTypeFrom(resultType);
            ThrowIfMissingQueryProvider(queryProviderType, queryType, resultType);
            return _container.Get(queryProviderType);
        }

        QueryProviderResult ExecuteOnProvider(object provider, object query, PagingInfo paging)
        {
            var method = provider.GetType().GetTypeInfo().GetMethod(ExecuteMethodName);
            var result = method.Invoke(provider, new[] { query, paging }) as QueryProviderResult;
            return result;
        }

        Type GetQueryTypeFrom(Type type)
        {
            var queryProviderForType = type.GetTypeInfo().GetInterface(typeof(IQueryProviderFor<>).FullName);
            var queryType = queryProviderForType.GetTypeInfo().GetGenericArguments()[0];
            return queryType;
        }

        Type GetActualProviderTypeFrom(Type type)
        {
            if (_queryProviderTypesPerTargetType.ContainsKey(type))
            {
                return _queryProviderTypesPerTargetType[type];
            }
            else
            {
                var interfaces = type.GetTypeInfo().GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    type = GetActualProviderTypeFrom(@interface);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        void DiscoverQueryTypesPerTargetType()
        {
            var queryTypes = _typeFinder.FindMultiple(typeof(IQueryProviderFor<>));

            _queryProviderTypesPerTargetType = queryTypes.Select(t => new
            {
                TargetType = GetQueryTypeFrom(t),
                QueryProviderType = t
            }).ToDictionary(t => t.TargetType, t => t.QueryProviderType);
        }
    }
}