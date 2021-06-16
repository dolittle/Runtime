// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dolittle.Runtime.Reflection;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Represents a builder for building <see cref="MessageDescription"/> for a specified type.
    /// </summary>
    /// <typeparam name="T">The type the builder is building.</typeparam>
    public class MessageDescriptionBuilderFor<T> : IMessageDescriptionBuilderFor<T>
    {
        readonly IEnumerable<IPropertyDescriptionBuilder> _propertyDescriptionBuilders;
        readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDescriptionBuilderFor{T}"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="propertyDescriptionBuilders"><see cref="IPropertyDescriptionBuilder">Property builders</see>.</param>
        public MessageDescriptionBuilderFor(string name, IEnumerable<IPropertyDescriptionBuilder> propertyDescriptionBuilders = null)
        {
            _propertyDescriptionBuilders = propertyDescriptionBuilders ?? Array.Empty<IPropertyDescriptionBuilder>();
            _name = name;
        }

        /// <inheritdoc/>
        public MessageDescription Build()
        {
            var properties = _propertyDescriptionBuilders.Select(_ => _.Build()).ToArray();
            return new MessageDescription(typeof(T), properties, _name);
        }

        /// <inheritdoc/>
        public IMessageDescriptionBuilderFor<T> Property<TProp>(Expression<Func<TProp>> property, Func<IPropertyDescriptionBuilder, IPropertyDescriptionBuilder> propertyDescriptionBuilderCallback)
        {
            var propertyInfo = property.GetPropertyInfo();
            IPropertyDescriptionBuilder propertyDescriptionBuilder = new PropertyDescriptionBuilder(propertyInfo, propertyInfo.Name, null, 0);
            propertyDescriptionBuilder = propertyDescriptionBuilderCallback(propertyDescriptionBuilder);
            var propertyDescriptionBuilders = new List<IPropertyDescriptionBuilder>(_propertyDescriptionBuilders)
            {
                propertyDescriptionBuilder
            };
            return new MessageDescriptionBuilderFor<T>(_name, propertyDescriptionBuilders);
        }

        /// <inheritdoc/>
        public IMessageDescriptionBuilderFor<T> WithAllProperties()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyDescriptionBuilders = properties.Select(_ => new PropertyDescriptionBuilder(_, _.Name, null, 0)).ToArray();
            return new MessageDescriptionBuilderFor<T>(_name, propertyDescriptionBuilders);
        }
    }
}