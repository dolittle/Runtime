/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Reflection;
using Dolittle.Serialization.Protobuf;
using Dolittle.Time;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Globalization;

namespace Dolittle.Runtime.Events.Relativity.Protobuf.Conversion
{
    /// <summary>
    /// Extension methods for working with conversion related to <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/>
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// Convert from a <see cref="System.Guid"/> to a <see cref="System.Protobuf.guid"/>
        /// </summary>
        /// <param name="guid"><see cref="System.Guid"/> to convert</param>
        /// <returns>A <see cref="System.Protobuf.guid"/></returns>
        public static System.Protobuf.guid ToProtobuf(this System.Guid guid)
        {
            var protobufGuid = new System.Protobuf.guid();
            protobufGuid.Value = ByteString.CopyFrom(guid.ToByteArray());
            return protobufGuid;
        }


        /// <summary>
        /// Convert a <see cref="ConceptAs{T}">Guid concept</see> to <see cref="System.Protobuf.guid"/>
        /// </summary>
        /// <param name="concept"><see cref="ConceptAs{T}"/> to convert from</param>
        /// <returns>Converted <see cref="System.Protobuf.guid"/></returns>
        public static System.Protobuf.guid ToProtobuf(this ConceptAs<Guid> concept)
        {
            return concept.Value.ToProtobuf();
        }


        /// <summary>
        /// Convert a <see cref="System.Protobuf.guid"/> to <see cref="ConceptAs{T}">Guid concept</see>
        /// </summary>
        /// <param name="guid"><see cref="System.Protobuf.guid"/> to convert from</param>
        /// <typeparam name="T">Type of <see cref="ConceptAs{T}"/> to convert to</typeparam>
        /// <returns>Converted instance - matching the type given</returns>
        public static T ToConcept<T>(this System.Protobuf.guid guid) where T : ConceptAs<Guid>
        {
            return ConceptFactory.CreateConceptInstance(typeof(T), new Guid(guid.Value.ToByteArray())) as T;
        }


        /// <summary>
        /// Get the protobuf <see cref="Types"/> representation of the <see cref="object">instances type</see>
        /// </summary>
        /// <param name="obj"><see cref="object">instance</see> to get from</param>
        /// <returns><see cref="Types">Protobuf type</see></returns>
        public static Types GetProtobufType(this object obj)
        {
            var type = obj.GetType();
            if( type == typeof(string)) return Types.String;
            if( type == typeof(int)) return Types.Int32;
            if( type == typeof(long)) return Types.Int64;
            if( type == typeof(uint)) return Types.UInt32;
            if( type == typeof(long)) return Types.UInt64;
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
        /// Read value from <see cref="System.Protobuf.Object"/>
        /// </summary>
        /// <param name="obj"><see cref="System.Protobuf.Object"/> to read from</param>
        /// <returns>Value in the correct type - null if not capable of converting</returns>
        public static object ToCLR(this System.Protobuf.Object obj)
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
                    case Types.DateTime: value = DateTime.FromFileTime(stream.ReadInt64()); break;
                    case Types.DateTimeOffset: value = DateTimeOffset.FromFileTime(stream.ReadInt64()); break;
                    case Types.Guid: value = new Guid(stream.ReadBytes().ToByteArray()); break;
                }
            }

            return value;
        }
    }
}