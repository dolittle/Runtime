/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doLittle.DependencyInversion;
using doLittle.Execution;
using doLittle.Collections;
using doLittle.Types;

namespace doLittle.Read.Validation
{
    /// <summary>
    /// Represents an implementation of <see cref="IQueryValidationDescriptors"/> 
    /// </summary>
    [Singleton]
    public class QueryValidationDescriptors : IQueryValidationDescriptors
    {
        Dictionary<Type, IQueryValidationDescriptor> _descriptors = new Dictionary<Type, IQueryValidationDescriptor>();

        /// <summary>
        /// Initializes an instance of <see cref="QueryValidationDescriptors"/>
        /// </summary>
        public QueryValidationDescriptors(ITypeFinder typeFinder, IContainer container)
        {
            var descriptors = typeFinder.FindMultiple(typeof(QueryValidationDescriptorFor<>)).Where(d => d != typeof(QueryValidationDescriptorFor<>));
            descriptors.ForEach(d => {
                var queryType = d.GetTypeInfo().BaseType.GetTypeInfo().GetGenericArguments()[0];
                var descriptor = container.Get(d) as IQueryValidationDescriptor;
                _descriptors[queryType] = descriptor;
            });
        }

        /// <inheritdoc/>
        public bool HasDescriptorFor<TQuery>() where TQuery : IQuery
        {
            return _descriptors.ContainsKey(typeof(TQuery)); 
        }

        /// <inheritdoc/>
        public IQueryValidationDescriptor GetDescriptorFor<TQuery>() where TQuery : IQuery
        {
            return _descriptors[typeof(TQuery)];
        }
    }
}
