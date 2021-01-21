// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Booting
{
    /// <summary>
    /// Defines the builder for <see cref="Boot"/>.
    /// </summary>
    public interface IBootBuilder
    {
        /// <summary>
        /// Set a value in a <see cref="IRepresentSettingsForBootStage">boot stage settings object</see>.
        /// </summary>
        /// <typeparam name="TTarget "><see cref="IRepresentSettingsForBootStage"/> type.</typeparam>
        /// <param name="propertyExpression">Expression to define the property to access.</param>
        /// <param name="value">Value to set.</param>
        void Set<TTarget>(Expression<Func<TTarget, object>> propertyExpression, object value)
            where TTarget : class, IRepresentSettingsForBootStage, new();

        /// <summary>
        /// Build the <see cref="Boot"/>.
        /// </summary>
        /// <returns>Built <see cref="Boot"/>.</returns>
        Boot Build();
    }
}
