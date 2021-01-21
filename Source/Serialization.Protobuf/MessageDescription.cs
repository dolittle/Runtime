// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.Serialization.Protobuf
{
    /// <summary>
    /// Represents the contract of a message.
    /// </summary>
    public class MessageDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDescription"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> representing the message.</param>
        /// <param name="properties"><see cref="IEnumerable{PropertyDescription}">Property descriptions</see>.</param>
        /// <param name="name">Name of the message.</param>
        public MessageDescription(Type type, IEnumerable<PropertyDescription> properties, string name = null)
        {
            Type = type;
            Properties = properties;
            Name = name ?? type.Name;
        }

        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the actual <see cref="Type">CLR type</see> representing the message.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the properties for the message.
        /// </summary>
        public IEnumerable<PropertyDescription> Properties { get; }

        /// <summary>
        /// Start building a description for.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to build for.</typeparam>
        /// <param name="builderCallback">Callback for building a <see cref="MessageDescription"/>.</param>
        /// <returns>A new instance of <see cref="MessageDescription"/> representing the type.</returns>
        public static MessageDescription For<T>(Func<IMessageDescriptionBuilderFor<T>, IMessageDescriptionBuilderFor<T>> builderCallback)
        {
            IMessageDescriptionBuilderFor<T> builder = new MessageDescriptionBuilderFor<T>(typeof(T).Name);
            builder = builderCallback(builder);
            return builder.Build();
        }

        /// <summary>
        /// Create a default <see cref="MessageDescription"/> with all the properties added by their hashcode.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to get default for.</typeparam>
        /// <returns>A new instance of <see cref="MessageDescription"/> representing the type.</returns>
        public static MessageDescription DefaultFor<T>()
        {
            IMessageDescriptionBuilderFor<T> builder = new MessageDescriptionBuilderFor<T>(typeof(T).Name);
            builder = builder.WithAllProperties();
            return builder.Build();
        }
    }
}