// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Types;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Represents an implementation of <see cref="IValueConverters"/>.
    /// </summary>
    public class ValueConverters : IValueConverters
    {
        readonly IInstancesOf<IValueConverter> _converters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverters"/> class.
        /// </summary>
        /// <param name="converters"><see cref="IInstancesOf{T}"/> <see cref="IValueConverter"/>.</param>
        public ValueConverters(IInstancesOf<IValueConverter> converters)
        {
            _converters = converters;
        }

        /// <inheritdoc/>
        public bool CanConvert(Type type)
        {
            return _converters.Any(_ => _.CanConvert(type));
        }

        /// <inheritdoc/>
        public IValueConverter GetConverterFor(Type type)
        {
            var valueConverter = _converters.FirstOrDefault(_ => _.CanConvert(type));
            ThrowIfMissingValueConverter(type, valueConverter);
            return valueConverter;
        }

        void ThrowIfMissingValueConverter(Type type, IValueConverter valueConverter)
        {
            if (valueConverter == null) throw new MissingValueConverter(type);
        }
    }
}