/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.Protobuf;
using Dolittle.Reflection;
using Dolittle.Time;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// Converts a <see cref="object"/> to <see cref="Value"/>.
        /// </summary>
        /// <remarks>
        /// This is primarily used when converting propertybags to protobuf messages and scenarios when we don't
        /// know the actual type of obj. 
        /// </remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Value ToProtobuf(this object obj)
        {
            var value = new Value();
            var objType = obj.GetType();

            if (objType.IsEnumerable() && !objType.IsDictionary()) value.ListValue = ((System.Collections.IEnumerable)obj).ToProtobuf();
            else if (objType == typeof(Dolittle.PropertyBags.PropertyBag)) value.DictionaryValue = ((Dolittle.PropertyBags.PropertyBag)obj).ToProtobuf().AsDictionaryValue();
            else 
            {
                var protobufObj = new Dolittle.Object();
                var protobufType = obj.GetProtobufType();
                protobufObj.Type = (int)protobufType;

                var stream = new MemoryStream();
                using( var outputStream = new CodedOutputStream(stream) )
                {
                    obj.WriteWithTypeTo(protobufType, outputStream);
                    outputStream.Flush();
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    protobufObj.Content = ByteString.CopyFrom(stream.ToArray());
                }
                value.ObjectValue = protobufObj;
                
            }
            return value;
        }

        /// <summary>
        /// Get the protobuf <see cref="Types"/> representation of the <see cref="object">instances type</see>
        /// </summary>
        /// <param name="obj"><see cref="object">instance</see> to get from</param>
        /// <returns><see cref="Types">Protobuf type</see></returns>
        public static Types GetProtobufType(this object obj) => obj.GetType().AsProtobufTypes();

        /// <summary>
        /// Gets the protobuf <see cref="Types"/> representation of the <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Types AsProtobufTypes(this Type type)
        {
            if( type == typeof(string)) return Types.String;
            if( type == typeof(int)) return Types.Int32;
            if( type == typeof(long)) return Types.Int64;
            if( type == typeof(uint)) return Types.UInt32;
            if( type == typeof(ulong)) return Types.UInt64;
            if( type == typeof(Int32)) return Types.Int32;
            if( type == typeof(Int64)) return Types.Int64;
            if( type == typeof(UInt32)) return Types.UInt32;
            if( type == typeof(UInt64)) return Types.UInt64;
            if( type == typeof(float)) return Types.Float;
            if( type == typeof(double)) return Types.Double;
            if( type == typeof(bool)) return Types.Boolean;
            if( type == typeof(DateTime)) return Types.DateTime;
            if( type == typeof(DateTimeOffset)) return Types.DateTimeOffset;
            if( type == typeof(Guid)) return Types.Guid;

            return Types.Unknown;
        }

        /// <summary>
        /// Write an <see cref="object"/> with <see cref="Types">type information</see> to a <see cref="CodedOutputStream"/>
        /// </summary>
        /// <param name="obj"><see cref="object"/> to write</param>
        /// <param name="type"><see cref="Types">Type</see> to write</param>
        /// <param name="stream"><see cref="CodedOutputStream"/> to write to</param>
        public static void WriteWithTypeTo(this object obj, Types type, CodedOutputStream stream)
        {
            switch( type )
            {
                case Types.String: stream.WriteString(obj as string); break;
                case Types.Int32: stream.WriteInt32((int)obj); break;
                case Types.Int64: stream.WriteInt64((Int64)obj); break;
                case Types.UInt32: stream.WriteUInt32((uint)obj); break;
                case Types.UInt64: stream.WriteUInt64((UInt64)obj); break;
                case Types.Float: stream.WriteFloat((float)obj); break;
                case Types.Double: stream.WriteDouble((double)obj); break;
                case Types.Boolean: stream.WriteBool((bool)obj); break;
                case Types.DateTime: stream.WriteInt64((Int64)((DateTime)obj).ToUnixTimeMilliseconds()); break;
                case Types.DateTimeOffset: stream.WriteInt64((Int64)((DateTimeOffset)obj).ToUnixTimeMilliseconds()); break;
                case Types.Guid: stream.WriteBytes(ByteString.CopyFrom(((Guid)obj).ToByteArray())); break;
            }
        }
        /// <summary>
        /// Read value from <see cref="Value"/>
        /// </summary>
        /// <param name="value"><see cref="Value"/> to read from</param>
        /// <returns>Value in the correct type - null if not capable of converting</returns>
        public static object ToCLR(this Value value)
        {
            object returnValue = null;

            switch ( value.KindCase )
            {
                case Value.KindOneofCase.ObjectValue:
                    returnValue = value.ObjectValue.ToCLR();
                    break;
                case Value.KindOneofCase.ListValue:
                    returnValue = value.ListValue.ToCLR();
                    break;
                case Value.KindOneofCase.DictionaryValue:
                    returnValue = value.DictionaryValue.ToCLR();
                    break;
            }

            return returnValue;   
        }
        /// <summary>
        /// Read value from <see cref="Object"/>
        /// </summary>
        /// <param name="obj"><see cref="Object"/> to read from</param>
        /// <returns>Value in the correct type - null if not capable of converting</returns>
        public static object ToCLR(this Object obj)
        {
            var type = (Types)obj.Type;
            object value = null;
            using( var stream = new CodedInputStream(obj.Content.ToByteArray()))
            {
                switch( type )
                {
                    case Types.String: value = stream.ReadString(); break;
                    case Types.Int32: value = stream.ReadInt32(); break;
                    case Types.Int64: value = stream.ReadInt64(); break;
                    case Types.UInt32: value = stream.ReadUInt32(); break;
                    case Types.UInt64: value = stream.ReadUInt64(); break;
                    case Types.Float: value = stream.ReadFloat(); break;
                    case Types.Double: value = stream.ReadDouble(); break;
                    case Types.Boolean: value = stream.ReadBool(); break;
                    case Types.DateTime: value = stream.ReadInt64().ToDateTime(); break;
                    case Types.DateTimeOffset: value = stream.ReadInt64().ToDateTimeOffset(); break;
                    case Types.Guid: value = new Guid(stream.ReadBytes().ToByteArray()); break;
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a <see cref="System.Collections.IEnumerable"/> to <see cref="ArrayValue"/>
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static ArrayValue ToProtobuf(this System.Collections.IEnumerable enumerable)
        {
            var arrayValue = new ArrayValue();
            foreach (var item in enumerable) arrayValue.Values.Add(item.ToProtobuf());
            return arrayValue; 
        }

        /// <summary>
        /// Read value from <see cref="ArrayValue"/>
        /// </summary>
        /// <param name="array"><see cref="ArrayValue"/> to read from</param>
        /// <returns>Array of values</returns>
        public static object[] ToCLR(this ArrayValue array)
        {
            var list = new List<object>();
            list.AddRange(array.Values.Select(val => val.ToCLR()));
            return list.ToArray();
        }

        /// <summary>
        /// Read value from <see cref="DictionaryValue"/>
        /// </summary>
        /// <param name="dictionary"><see cref="DictionaryValue"/> to read from</param>
        /// <returns>The <see cref="Dolittle.PropertyBags.PropertyBag"/></returns>
        public static Dolittle.PropertyBags.PropertyBag ToCLR(this DictionaryValue dictionary) => dictionary.Object.ToCLR();

        /// <summary>
        /// Read value from <see cref="MapField{TKey, TValue}"/>
        /// </summary>
        /// <param name="propertyBag"><see cref="MapField{TKey, TValue}"/> to read from</param>
        /// <returns>The <see cref="Dolittle.PropertyBags.PropertyBag"/></returns>
        public static Dolittle.PropertyBags.PropertyBag ToCLR(this MapField<string, Value> propertyBag)
        {
            var nullFreedictionary = new NullFreeDictionary<string,object>();
            propertyBag.ForEach(keyValue => 
            {
                var value = keyValue.Value.ToCLR();
                if(value != null) nullFreedictionary.Add(keyValue.Key, value);
            });
            return new Dolittle.PropertyBags.PropertyBag(nullFreedictionary);
        }
    }
}