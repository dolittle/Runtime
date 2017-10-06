/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using doLittle.Types;
using doLittle.DependencyInversion;

namespace doLittle.Execution
{
    /// <summary>
    /// Represents a <see cref="IExecutionContextDetailsPopulator"/>
    /// </summary>
    public class ExecutionContextDetailsPopulator : IExecutionContextDetailsPopulator
    {
        ITypeFinder _typeFinder;
        IContainer _container;
        IEnumerable<Type> _populatorTypes;

        /// <summary>
        /// Initializes an instance of <see cref="ExecutionContextDetailsPopulator"/>
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for discovering implementations of <see cref="ICanPopulateExecutionContextDetails"/></param>
        /// <param name="container"><see cref="IContainer"/> to use for instantiating types</param>
        public ExecutionContextDetailsPopulator(ITypeFinder typeFinder, IContainer container)
        {
            _typeFinder = typeFinder;
            _container = container;
            _populatorTypes = _typeFinder.FindMultiple<ICanPopulateExecutionContextDetails>();
        }

        /// <inheritdoc/>
        public void Populate(IExecutionContext executionContext, dynamic details)
        {
            foreach (var type in _populatorTypes)
            {
                var instance = _container.Get(type) as ICanPopulateExecutionContextDetails;
                instance.Populate(executionContext, details);
            }
        }
    }
}
