/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Collections;
using Dolittle.Runtime.Events.Relativity.Interaction.Grpc;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// Extensions for converting <see cref="PropertyBag"/> to and from protobuf representations
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Convert from <see cref="PropertyBags.PropertyBag"/> to a <see cref="PropertyBag"/>
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert from</param>
        /// <returns>Converted <see cref="PropertyBag"/></returns>
        public static PropertyBag ToProtobuf(this PropertyBags.PropertyBag propertyBag)
        {
            var protobufPropertyBag = new PropertyBag();
            propertyBag.ForEach(kvp => protobufPropertyBag.Values.Add(kvp.Key, kvp.Value.ToProtobuf()));
            return protobufPropertyBag;
        }

        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to <see cref="PropertyBags.PropertyBag"/>
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert from</param>
        /// <returns>Converted <see cref="PropertyBags.PropertyBag"/></returns>
        public static PropertyBags.PropertyBag ToPropertyBag(this PropertyBag propertyBag) => propertyBag.Values.ToCLR();
        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to <see cref="System.Protobuf.DictionaryValue"/>
        /// </summary>
        /// <param name="propertyBag"></param>
        /// <returns></returns>
        public static System.Protobuf.DictionaryValue AsDictionaryValue(this PropertyBag propertyBag)
        {
            var dictionaryValue = new System.Protobuf.DictionaryValue();
            propertyBag.Values.ForEach(kvp => dictionaryValue.Object.Add(kvp.Key, kvp.Value));
            return dictionaryValue;
        }
        /// <summary>
        /// Convert from <see cref="System.Protobuf.DictionaryValue"/> to <see cref="PropertyBag"/>
        /// </summary>
        /// <param name="dictionaryValue"></param>
        /// <returns></returns>
        public static PropertyBag AsPropertyBag(this System.Protobuf.DictionaryValue dictionaryValue)
        {
            var propertyBag = new PropertyBag();
            dictionaryValue.Object.ForEach(kvp => propertyBag.Values.Add(kvp.Key, kvp.Value));
            return propertyBag;
        }

    }
}