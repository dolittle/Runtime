// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.Immutability;

/// <summary>
/// Exception that gets thrown when an <see cref="IAmImmutable">immutable object</see> is mutable
/// by virtue of it having fields that can be written to.
/// </summary>
public class WriteableImmutableFieldsFound : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WriteableImmutableFieldsFound"/> class.
    /// </summary>
    /// <param name="type"><see cref="Type"/> with immutable fields on.</param>
    /// <param name="fields"><see cref="IEnumerable{T}"/> of <see cref="FieldInfo">fields</see>.</param>
    public WriteableImmutableFieldsFound(Type type, IEnumerable<FieldInfo> fields)
        : base($"Type '{type.AssemblyQualifiedName}' has writeable properties called '{string.Join(",", fields.Select(_ => _.Name))}' - this is not allowed for immutable objects. Mark them with readonly.")
    {
    }
}