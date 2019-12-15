// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries.Validation
{
    /// <summary>
    /// Defines a system for accessing validation descriptors for queries.
    /// </summary>
    public interface IQueryValidationDescriptors
    {
        /// <summary>
        /// Checks if there is a <see cref="QueryValidationDescriptorFor{TQ}"/> for a specific <see cref="IQuery"/> by its type.
        /// </summary>
        /// <typeparam name="TQuery">Type of <see cref="IQuery"/> to check for.</typeparam>
        /// <returns>True if there is a <see cref="QueryValidationDescriptorFor{TQ}"/> for the query and false if not.</returns>
        bool HasDescriptorFor<TQuery>()
            where TQuery : IQuery;

        /// <summary>
        /// Get a <see cref="QueryValidationDescriptorFor{TQ}"/> for a specific <see cref="IQuery"/> by its type.
        /// </summary>
        /// <typeparam name="TQuery">Type of <see cref="IQuery"/> to get for.</typeparam>
        /// <returns><see cref="IQueryValidationDescriptor"/> describing the validation for the <see cref="IQuery"/>.</returns>
        IQueryValidationDescriptor GetDescriptorFor<TQuery>()
            where TQuery : IQuery;
    }
}
