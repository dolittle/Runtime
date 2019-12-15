// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Collections;
using Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting <see cref="PropertyBag"/> to and from protobuf representations.
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to a <see cref="PropertyBag"/>.
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert from.</param>
        /// <returns>Converted <see cref="PropertyBag"/>.</returns>
        public static PropertyBag ToProtobuf(this PropertyBags.PropertyBag propertyBag)
        {
            var protobufPropertyBag = new PropertyBag();
            propertyBag.ForEach(kvp => protobufPropertyBag.Values.Add(kvp.Key, kvp.Value.ToProtobuf()));
            return protobufPropertyBag;
        }

        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to <see cref="PropertyBag"/>.
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert from.</param>
        /// <returns>Converted <see cref="PropertyBag"/>.</returns>
        public static PropertyBags.PropertyBag ToPropertyBag(this PropertyBag propertyBag) => propertyBag.Values.ToCLR();

        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to <see cref="DictionaryValue"/>.
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert.</param>
        /// <returns>Converted <see cref="DictionaryValue"/>.</returns>
        public static DictionaryValue AsDictionaryValue(this PropertyBag propertyBag)
        {
            var dictionaryValue = new DictionaryValue();
            propertyBag.Values.ForEach(kvp => dictionaryValue.Object.Add(kvp.Key, kvp.Value));
            return dictionaryValue;
        }

        /// <summary>
        /// Convert from <see cref="DictionaryValue"/> to <see cref="PropertyBag"/>.
        /// </summary>
        /// <param name="dictionaryValue"><see cref="DictionaryValue"/> to convert.</param>
        /// <returns>Converted <see cref="PropertyBag"/>.</returns>
        public static PropertyBag AsPropertyBag(this DictionaryValue dictionaryValue)
        {
            var propertyBag = new PropertyBag();
            dictionaryValue.Object.ForEach(kvp => propertyBag.Values.Add(kvp.Key, kvp.Value));
            return propertyBag;
        }
    }
}