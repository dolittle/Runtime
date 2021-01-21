// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.Immutability
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IAmImmutable">immutable object</see> is mutable
    /// by virtue of it having properties that can be written to.
    /// </summary>
    public class WriteableImmutablePropertiesFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableImmutablePropertiesFound"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> with immutable fields on.</param>
        /// <param name="properties"><see cref="IEnumerable{T}"/> of <see cref="PropertyInfo">properties</see>.</param>
        public WriteableImmutablePropertiesFound(Type type, IEnumerable<PropertyInfo> properties)
            : base($"Type '{type.AssemblyQualifiedName}' has writeable properties called '{string.Join(",", properties.Select(_ => _.Name))}' - this is not allowed for immutable objects")
        {
        }
    }
}