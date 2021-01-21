// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Serialization.Protobuf;

namespace Dolittle.Runtime.Concepts.Serialization.Protobuf
{
    /// <summary>
    /// Represents an implementation of <see cref="IValueConverter"/> that deals with serializing and deserialing of <see cref="ConceptAs{T}"/>.
    /// </summary>
    public class ConceptConverter : IValueConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(Type objectType)
        {
            return objectType.IsConcept();
        }

        /// <inheritdoc/>
        public Type SerializedAs(Type objectType)
        {
            return objectType.GetConceptValueType();
        }

        /// <inheritdoc/>
        public object ConvertTo(object value)
        {
            return value.GetConceptValue();
        }

        /// <inheritdoc/>
        public object ConvertFrom(Type objectType, object value)
        {
            return ConceptFactory.CreateConceptInstance(objectType, value);
        }
    }
}