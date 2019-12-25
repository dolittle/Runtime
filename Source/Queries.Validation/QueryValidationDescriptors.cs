// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Represents an implementation of <see cref="IQueryValidationDescriptors"/>.
    /// </summary>
    [Singleton]
    public class QueryValidationDescriptors : IQueryValidationDescriptors
    {
        readonly Dictionary<Type, IQueryValidationDescriptor> _descriptors = new Dictionary<Type, IQueryValidationDescriptor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryValidationDescriptors"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for finding types.</param>
        /// <param name="container"><see cref="IContainer"/> for getting instances.</param>
        public QueryValidationDescriptors(ITypeFinder typeFinder, IContainer container)
        {
            var descriptors = typeFinder.FindMultiple(typeof(QueryValidationDescriptorFor<>)).Where(d => d != typeof(QueryValidationDescriptorFor<>));
            descriptors.ForEach(d =>
            {
                var queryType = d.GetTypeInfo().BaseType.GetTypeInfo().GetGenericArguments()[0];
                var descriptor = container.Get(d) as IQueryValidationDescriptor;
                _descriptors[queryType] = descriptor;
            });
        }

        /// <inheritdoc/>
        public bool HasDescriptorFor<TQuery>()
            where TQuery : IQuery
        {
            return _descriptors.ContainsKey(typeof(TQuery));
        }

        /// <inheritdoc/>
        public IQueryValidationDescriptor GetDescriptorFor<TQuery>()
            where TQuery : IQuery
        {
            return _descriptors[typeof(TQuery)];
        }
    }
}
