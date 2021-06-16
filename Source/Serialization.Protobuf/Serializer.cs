// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using Dolittle.Runtime.Collections;
using Google.Protobuf;
using static Google.Protobuf.WireFormat;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Represents an implementation of <see cref="ISerializer"/>.
    /// </summary>
    public class Serializer : ISerializer
    {
        readonly IMessageDescriptions _messageDescriptions;
        readonly IValueConverters _valueConverters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer"/> class.
        /// </summary>
        /// <param name="messageDescriptions"><see cref="IMessageDescriptions"/> for <see cref="MessageDescription">descriptions of messages</see>.</param>
        /// <param name="valueConverters">Available <see cref="IValueConverters"/>.</param>
        public Serializer(IMessageDescriptions messageDescriptions, IValueConverters valueConverters)
        {
            _messageDescriptions = messageDescriptions;
            _valueConverters = valueConverters;
        }

        /// <inheritdoc/>
        public int GetLengthOf<T>(T instance)
        {
            var messageDescription = _messageDescriptions.GetFor<T>();
            var length = GetLengthOf(instance, messageDescription);
            return length;
        }

        /// <inheritdoc/>
        public T FromProtobuf<T>(Stream stream, bool includesLength = false)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            using (var inputStream = new CodedInputStream(stream))
            {
                var messageDescription = _messageDescriptions.GetFor<T>();

                var tag = inputStream.ReadTag();
                while (!inputStream.IsAtEnd)
                {
                    var fieldNumber = WireFormat.GetTagFieldNumber(tag);
                    var propertyDescription = messageDescription.Properties.SingleOrDefault(_ => _.Number == fieldNumber);
                    if (propertyDescription != null)
                    {
                        object value = null;
                        var type = propertyDescription.Property.PropertyType;

                        IValueConverter converter = null;

                        var targetType = type;

                        if (_valueConverters.CanConvert(type))
                        {
                            converter = _valueConverters.GetConverterFor(type);
                            targetType = type;
                            type = converter.SerializedAs(type);
                        }

                        value = ReadValue(inputStream, value, type, targetType, converter);
                        propertyDescription.Property.SetValue(instance, value);
                    }

                    tag = inputStream.ReadTag();
                }
            }

            return instance;
        }

        /// <inheritdoc/>
        public T FromProtobuf<T>(byte[] bytes, bool includesLength = false)
        {
            using var memoryStream = new MemoryStream(bytes);
            return FromProtobuf<T>(memoryStream, includesLength);
        }

        /// <inheritdoc/>
        public void ToProtobuf<T>(T instance, Stream stream, bool includeLength = false)
        {
            using var outputStream = new CodedOutputStream(stream);
            var messageDescription = _messageDescriptions.GetFor<T>();

            if (includeLength)
            {
                var length = GetLengthOf(instance, messageDescription);
                outputStream.WriteLength(length);
            }

            messageDescription.Properties.ForEach(property =>
            {
                var type = property.Property.PropertyType;
                var number = property.Number;
                var value = property.Property.GetValue(instance);

                if (_valueConverters.CanConvert(type))
                {
                    var converter = _valueConverters.GetConverterFor(type);
                    type = converter.SerializedAs(type);
                    value = converter.ConvertTo(value);
                }

                WriteValue(outputStream, type, number, value);
            });
            outputStream.Flush();
        }

        /// <inheritdoc/>
        public byte[] ToProtobuf<T>(T instance, bool includeLength = false)
        {
            using var stream = new MemoryStream();
            ToProtobuf(instance, stream, includeLength);
            return stream.ToArray();
        }

        object ReadValue(CodedInputStream inputStream, object value, Type type, Type targetType, IValueConverter converter)
        {
            if (type == typeof(Guid))
            {
                _ = inputStream.ReadLength();
                var guidAsBytes = inputStream.ReadBytes();
                value = new Guid(guidAsBytes.ToByteArray());
            }
            else if (type == typeof(string))
            {
                _ = inputStream.ReadLength();
                value = inputStream.ReadString();
            }
            else if (type == typeof(int))
            {
                value = inputStream.ReadInt32();
            }
            else if (type == typeof(long))
            {
                value = inputStream.ReadInt64();
            }
            else if (type == typeof(uint))
            {
                value = inputStream.ReadUInt32();
            }
            else if (type == typeof(ulong))
            {
                value = inputStream.ReadUInt64();
            }
            else if (type == typeof(float))
            {
                value = inputStream.ReadFloat();
            }
            else if (type == typeof(double))
            {
                value = inputStream.ReadDouble();
            }
            else if (type == typeof(bool))
            {
                value = inputStream.ReadBool();
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTime))
            {
                value = DateTimeOffset.FromUnixTimeMilliseconds(inputStream.ReadInt64());
                if (type == typeof(DateTime)) value = ((DateTimeOffset)value).UtcDateTime;
            }

            if (converter != null) value = converter.ConvertFrom(targetType, value);
            return value;
        }

        void WriteValue(CodedOutputStream outputStream, Type type, int number, object value)
        {
            if (type == typeof(Guid))
            {
                outputStream.WriteTag(number, WireType.LengthDelimited);
                var guidAsBytes = ((Guid)value).ToByteArray();
                var byteString = ByteString.CopyFrom(guidAsBytes);
                outputStream.WriteLength(CodedOutputStream.ComputeBytesSize(byteString));
                outputStream.WriteBytes(byteString);
            }
            else if (type == typeof(string))
            {
                var valueAsString = value as string;
                outputStream.WriteTag(number, WireType.LengthDelimited);
                outputStream.WriteLength(CodedOutputStream.ComputeStringSize(valueAsString));
                outputStream.WriteString(valueAsString);
            }
            else if (type == typeof(int))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteInt32((int)value);
            }
            else if (type == typeof(long))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteInt64((long)value);
            }
            else if (type == typeof(uint))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteUInt32((uint)value);
            }
            else if (type == typeof(ulong))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteUInt64((ulong)value);
            }
            else if (type == typeof(float))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteFloat((float)value);
            }
            else if (type == typeof(double))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteDouble((double)value);
            }
            else if (type == typeof(bool))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteBool((bool)value);
            }
            else if (type == typeof(DateTimeOffset))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteInt64(((DateTimeOffset)value).ToUnixTimeMilliseconds());
            }
            else if (type == typeof(DateTime))
            {
                outputStream.WriteTag(number, WireType.Varint);
                outputStream.WriteInt64(new DateTimeOffset(((DateTime)value).ToUniversalTime()).ToUnixTimeMilliseconds());
            }
        }

        int GetLengthOf<T>(T instance, MessageDescription messageDescription)
        {
            var size = 0;

            messageDescription.Properties.ForEach(property =>
            {
                var type = property.Property.PropertyType;
                var number = property.Number;
                var value = property.Property.GetValue(instance);

                if (_valueConverters.CanConvert(type))
                {
                    var converter = _valueConverters.GetConverterFor(type);
                    type = converter.SerializedAs(type);
                    value = converter.ConvertTo(value);
                }

                size += CodedOutputStream.ComputeTagSize(number);
                if (type == typeof(Guid))
                {
                    var guidAsBytes = ((Guid)value).ToByteArray();
                    var length = CodedOutputStream.ComputeBytesSize(ByteString.CopyFrom(guidAsBytes));
                    size += CodedOutputStream.ComputeLengthSize(length);
                    size += length;
                }
                else if (type == typeof(string))
                {
                    var valueAsString = value as string;
                    var length = CodedOutputStream.ComputeStringSize(valueAsString);
                    size += CodedOutputStream.ComputeLengthSize(length);
                    size += length;
                }
                else if (type == typeof(int))
                {
                    size += CodedOutputStream.ComputeInt32Size((int)value);
                }
                else if (type == typeof(long))
                {
                    size += CodedOutputStream.ComputeInt64Size((long)value);
                }
                else if (type == typeof(uint))
                {
                    size += CodedOutputStream.ComputeUInt32Size((uint)value);
                }
                else if (type == typeof(ulong))
                {
                    size += CodedOutputStream.ComputeUInt64Size((ulong)value);
                }
                else if (type == typeof(float))
                {
                    size += CodedOutputStream.ComputeFloatSize((float)value);
                }
                else if (type == typeof(double))
                {
                    size += CodedOutputStream.ComputeDoubleSize((double)value);
                }
                else if (type == typeof(bool))
                {
                    size += CodedOutputStream.ComputeBoolSize((bool)value);
                }
                else if (type == typeof(DateTimeOffset))
                {
                    size += CodedOutputStream.ComputeInt64Size(((DateTimeOffset)value).ToUnixTimeMilliseconds());
                }
                else if (type == typeof(DateTime))
                {
                    size += CodedOutputStream.ComputeInt64Size(new DateTimeOffset(((DateTime)value).ToUniversalTime()).ToUnixTimeMilliseconds());
                }
            });

            return size;
        }
    }
}
