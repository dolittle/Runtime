// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dolittle.ReadModels
{
    /// <summary>
    /// Represents an implementation of <see cref="IReadModelOf{T}"/> for dealing with fetching of single <see cref="IReadModel"/> instances.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="IReadModel"/>.</typeparam>
    public class ReadModelOf<T> : IReadModelOf<T>
        where T : IReadModel
    {
        readonly IReadModelRepositoryFor<T> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadModelOf{T}"/> class.
        /// </summary>
        /// <param name="repository">Repository to use getting instances.</param>
        public ReadModelOf(IReadModelRepositoryFor<T> repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public T InstanceMatching(params Expression<Func<T, bool>>[] propertyExpressions)
        {
            var query = _repository.Query;

            foreach (var expression in propertyExpressions)
                query = query.Where(expression);

            return query.SingleOrDefault();
        }
    }
}
