// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

#pragma warning disable DL0008

namespace Dolittle.Runtime.Reflection;

/// <summary>
/// Exception that gets thrown when object being converted is null.
/// </summary>
public class SourceObjectCannotBeNull : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceObjectCannotBeNull"/> class.
    /// </summary>
    public SourceObjectCannotBeNull()
        : base("Unable to convert object to a dictionary. The source object is null.")
    {
    }
}