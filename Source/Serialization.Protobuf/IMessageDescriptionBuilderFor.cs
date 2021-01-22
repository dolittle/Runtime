// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Defines a builder for building <see cref="MessageDescription"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> the builder is for.</typeparam>
    public interface IMessageDescriptionBuilderFor<T>
    {
        /// <summary>
        /// Start building a <see cref="PropertyDescription">description</see> for a property.
        /// </summary>
        /// <typeparam name="TProp">Type of property.</typeparam>
        /// <param name="property"><see cref="Expression"/> representing a reference to the property to build for.</param>
        /// <param name="propertyDescriptionBuilderCallback">Callback that gets called with the <see cref="IPropertyDescriptionBuilder"/>.</param>
        /// <returns>A continuation of the <see cref="IMessageDescriptionBuilderFor{T}"/>.</returns>
        IMessageDescriptionBuilderFor<T> Property<TProp>(Expression<Func<TProp>> property, Func<IPropertyDescriptionBuilder, IPropertyDescriptionBuilder> propertyDescriptionBuilderCallback);

        /// <summary>
        /// Add all properties as default configuration.
        /// </summary>
        /// <returns>A continuation of the <see cref="IMessageDescriptionBuilderFor{T}"/>.</returns>
        IMessageDescriptionBuilderFor<T> WithAllProperties();

        /// <summary>
        /// Builds a completed <see cref="MessageDescription"/>.
        /// </summary>
        /// <returns><see cref="MessageDescription"/>.</returns>
        MessageDescription Build();
    }
}