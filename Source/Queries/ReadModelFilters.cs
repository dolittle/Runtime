/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.DependencyInversion;
using Dolittle.Types;
using Dolittle.ReadModels;

namespace Dolittle.Queries
{
    /// <summary>
    /// Represents an implementation of <see cref="IReadModelFilters"/>
    /// </summary>
    public class ReadModelFilters : IReadModelFilters
    {
        IContainer _container;
        IEnumerable<Type> _filterTypes;

        /// <summary>
        /// Initializes an instance of <see cref="ReadModelFilters"/>
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for discovering filters</param>
        /// <param name="container"><see cref="IContainer"/> for instantiating filters</param>
        public ReadModelFilters(ITypeFinder typeFinder, IContainer container)
        {
            _container = container;

            _filterTypes = typeFinder.FindMultiple<ICanFilterReadModels>();
        }

        /// <inheritdoc/>
        public IEnumerable<IReadModel> Filter(IEnumerable<IReadModel> readModels)
        {
            if (_filterTypes.Count() == 0) return readModels;

            foreach (var filterType in _filterTypes)
            {
                var filter = _container.Get(filterType) as ICanFilterReadModels;
                readModels = filter.Filter(readModels);
            }

            return readModels;
        }
    }
}
