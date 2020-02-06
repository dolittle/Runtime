// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolittle.Collections;
using Dolittle.Reflection;
using Dolittle.Time;
using Google.Protobuf;
using Google.Protobuf.Collections;
using grpc = Dolittle.Events.Relativity.Microservice;

namespace Dolittle.Runtime.Protobuf
{
    /// <summary>
    /// General extensions for converting to and back from protobuf.
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// Converts a <see cref="object"/> to <see cref="grpc.Value"/>.
        /// </summary>
        /// <param name="source"><see cref="object"/> to convert.</param>
        /// <returns>Converted value.</returns>
        public static grpc.Value ToProtobuf(this object source)
        {
            var value = new grpc.Value();
            var objType = source.GetType();

            if (objType.IsEnumerable() && !objType.IsDictionary())
            {
                value.ListValue = ((IEnumerable)source).ToProtobuf();
            }
            else
            {
                var protobufObj = new grpc.Object();
                var protobufType = source.GetProtobufType();
                protobufObj.Type = (int)protobufType;

                var stream = new MemoryStream();
                using (var outputStream = new CodedOutputStream(stream))
                {
                    source.WriteWithTypeTo(protobufType, outputStream);
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
        /// Get the protobuf <see cref="Types"/> representation of the <see cref="object">instances type</see>.
        /// </summary>
        /// <param name="source"><see cref="object">instance</see> to get from.</param>
        /// <returns><see cref="Types">Protobuf type</see>.</returns>
        public static Types GetProtobufType(this object source) => source.GetType().AsProtobufTypes();

        /// <summary>
        /// Gets the protobuf <see cref="Types"/> representation of the <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Type to get protobuf type for.</param>
        /// <returns><see cref="Types"/> representation.</returns>
        public static Types AsProtobufTypes(this Type type)
        {
            if (type == typeof(string)) return Types.String;
            if (type == typeof(int)) return Types.Int32;
            if (type == typeof(long)) return Types.Int64;
            if (type == typeof(uint)) return Types.UInt32;
            if (type == typeof(ulong)) return Types.UInt64;
            if (type == typeof(float)) return Types.Float;
            if (type == typeof(double)) return Types.Double;
            if (type == typeof(bool)) return Types.Boolean;
            if (type == typeof(DateTime)) return Types.DateTime;
            if (type == typeof(DateTimeOffset)) return Types.DateTimeOffset;
            if (type == typeof(Guid)) return Types.Guid;

            return Types.Unknown;
        }

        /// <summary>
        /// Write an <see cref="object"/> with <see cref="Types">type information</see> to a <see cref="CodedOutputStream"/>.
        /// </summary>
        /// <param name="source"><see cref="object"/> to write.</param>
        /// <param name="type"><see cref="Types">Type</see> to write.</param>
        /// <param name="stream"><see cref="CodedOutputStream"/> to write to.</param>
        public static void WriteWithTypeTo(this object source, Types type, CodedOutputStream stream)
        {
            switch (type)
            {
                case Types.String: stream.WriteString(source as string); break;
                case Types.Int32: stream.WriteInt32((int)source); break;
                case Types.Int64: stream.WriteInt64((long)source); break;
                case Types.UInt32: stream.WriteUInt32((uint)source); break;
                case Types.UInt64: stream.WriteUInt64((ulong)source); break;
                case Types.Float: stream.WriteFloat((float)source); break;
                case Types.Double: stream.WriteDouble((double)source); break;
                case Types.Boolean: stream.WriteBool((bool)source); break;
                case Types.DateTime: stream.WriteInt64(((DateTime)source).ToUnixTimeMilliseconds()); break;
                case Types.DateTimeOffset: stream.WriteInt64(((DateTimeOffset)source).ToUnixTimeMilliseconds()); break;
                case Types.Guid: stream.WriteBytes(ByteString.CopyFrom(((Guid)source).ToByteArray())); break;
            }
        }

        /// <summary>
        /// Read value from <see cref="grpc.Value"/>.
        /// </summary>
        /// <param name="value"><see cref="grpc.Value"/> to read from.</param>
        /// <returns>Value in the correct type - null if not capable of converting.</returns>
        public static object ToCLR(this grpc.Value value)
        {
            switch (value.KindCase)
            {
                case grpc.Value.KindOneofCase.ObjectValue:
                    return value.ObjectValue.ToCLR();
                case grpc.Value.KindOneofCase.ListValue:
                    return value.ListValue.ToCLR();
                case grpc.Value.KindOneofCase.DictionaryValue:
                    return value.DictionaryValue.ToCLR();
            }

            return null;
        }

        /// <summary>
        /// Read value from <see cref="grpc.Object"/>.
        /// </summary>
        /// <param name="source"><see cref="grpc.Object"/> to read from.</param>
        /// <returns>Value in the correct type - null if not capable of converting.</returns>
        public static object ToCLR(this grpc.Object source)
        {
            var type = (Types)source.Type;
            object value = null;
            using (var stream = new CodedInputStream(source.Content.ToByteArray()))
            {
                switch (type)
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
        /// Converts a <see cref="IEnumerable"/> to <see cref="grpc.ArrayValue"/>.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable"/> to convert.</param>
        /// <returns>Converted <see cref="grpc.ArrayValue"/>.</returns>
        public static grpc.ArrayValue ToProtobuf(this IEnumerable enumerable)
        {
            var arrayValue = new grpc.ArrayValue();
            foreach (var item in enumerable) arrayValue.Values.Add(item.ToProtobuf());
            return arrayValue;
        }

        /// <summary>
        /// Read value from <see cref="grpc.ArrayValue"/>.
        /// </summary>
        /// <param name="array"><see cref="grpc.ArrayValue"/> to read from.</param>
        /// <returns>Array of values.</returns>
        public static object[] ToCLR(this grpc.ArrayValue array)
        {
            var list = new List<object>();
            list.AddRange(array.Values.Select(val => val.ToCLR()));
            return list.ToArray();
        }

        /// <summary>
        /// Read value from <see cref="grpc.DictionaryValue"/>.
        /// </summary>
        /// <param name="dictionary"><see cref="grpc.DictionaryValue"/> to read from.</param>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public static IDictionary<string, object> ToCLR(this grpc.DictionaryValue dictionary) => dictionary.Object.ToCLR();

        /// <summary>
        /// Read value from <see cref="MapField{TKey, TValue}"/>.
        /// </summary>
        /// <param name="keysAndValues"><see cref="MapField{TKey, TValue}"/> to read from.</param>
        /// <returns>The <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public static IDictionary<string, object> ToCLR(this MapField<string, grpc.Value> keysAndValues)
        {
            var nullFreedictionary = new NullFreeDictionary<string, object>();
            keysAndValues.ForEach(keyAndValue =>
            {
                var value = keyAndValue.Value.ToCLR();
                if (value != null) nullFreedictionary.Add(keyAndValue.Key, value);
            });
            return nullFreedictionary;
        }
    }
}