/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Linq;
using Dolittle.Collections;
using Dolittle.PropertyBags;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extensions for converting <see cref="PropertyBag"/> to and from protobuf representations
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Convert from <see cref="PropertyBag"/> to a <see cref="MapField{key,value}"/>
        /// </summary>
        /// <param name="propertyBag"><see cref="PropertyBag"/> to convert from</param>
        /// <returns>Converted <see cref="MapField{key,value}"/></returns>
        public static MapField<string, System.Protobuf.Object> ToProtobuf(this PropertyBag propertyBag)
        {
            var mapField = new MapField<string, System.Protobuf.Object>();
            propertyBag.ForEach(keyValue => 
            {
                var obj = new System.Protobuf.Object();
                var type = keyValue.Value.GetProtobufType();
                obj.Type = (int)type;

                var stream = new MemoryStream();
                using( var outputStream = new CodedOutputStream(stream) )
                {
                    keyValue.Value.WriteWithTypeTo(type, outputStream);
                    outputStream.Flush();
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    obj.Content = ByteString.CopyFrom(stream.ToArray());
                }
                
                mapField.Add(keyValue.Key, obj);
            });

            return mapField;
        }

        /// <summary>
        /// Convert from <see cref="MapField{key,value}"/> to <see cref="PropertyBag"/>
        /// </summary>
        /// <param name="mapField"><see cref="MapField{key,value}"/> to convert from</param>
        /// <returns>Converted <see cref="PropertyBag"/></returns>
        public static PropertyBag ToPropertyBag(this MapField<string, System.Protobuf.Object> mapField)
        {
            var dictionary = new NullFreeDictionary<string,object>();
            mapField.ForEach(keyValue => 
            {
                var value = keyValue.Value.ConvertToCLR();
                if(value != null) dictionary.Add(keyValue.Key, value);
            });
            return new PropertyBag(dictionary);
        }
    }
}